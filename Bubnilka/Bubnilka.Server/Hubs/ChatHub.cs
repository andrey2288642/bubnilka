using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Bubnilka.Server.Data;
using Bubnilka.Shared.Models;
using System.Security.Claims;

namespace Bubnilka.Server.Hubs;

public class ChatHub : Hub
{
    private readonly AppDbContext _db;
    private readonly ILogger<ChatHub> _logger;

    public ChatHub(AppDbContext db, ILogger<ChatHub> logger)
    {
        _db = db;
        _logger = logger;
    }

    public override async Task OnConnectedAsync()
    {
        var userId = GetUserId();
        if (userId > 0)
        {
            var user = await _db.Users.FindAsync(userId);
            if (user != null)
            {
                user.IsOnline = true;
                user.LastSeen = DateTime.UtcNow;
                await _db.SaveChangesAsync();
                
                await Clients.All.SendAsync("UserStatusChanged", userId, true);
            }
        }
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = GetUserId();
        if (userId > 0)
        {
            var user = await _db.Users.FindAsync(userId);
            if (user != null)
            {
                user.IsOnline = false;
                user.LastSeen = DateTime.UtcNow;
                await _db.SaveChangesAsync();
                
                await Clients.All.SendAsync("UserStatusChanged", userId, false);
            }
        }
        await base.OnDisconnectedAsync(exception);
    }

    public async Task SendMessage(int chatId, string content)
    {
        var userId = GetUserId();
        if (userId <= 0) return;

        var chat = await _db.Chats
            .FirstOrDefaultAsync(c => 
                c.Id == chatId && 
                (c.UserId1 == userId || c.UserId2 == userId));

        if (chat == null) return;

        var message = new Message
        {
            ChatId = chatId,
            SenderId = userId,
            Content = content,
            SentAt = DateTime.UtcNow,
            IsRead = false
        };

        _db.Messages.Add(message);
        chat.LastMessageAt = message.SentAt;
        await _db.SaveChangesAsync();

        // Reload message with sender info
        await _db.Entry(message).Reference(m => m.Sender).LoadAsync();

        var messageDto = new
        {
            message.Id,
            message.ChatId,
            message.SenderId,
            message.Content,
            message.SentAt,
            SenderName = message.Sender.DisplayName
        };

        await Clients.Group($"chat-{chatId}").SendAsync("ReceiveMessage", messageDto);
    }

    public async Task JoinChat(int chatId)
    {
        var userId = GetUserId();
        if (userId <= 0) return;

        var chat = await _db.Chats
            .FirstOrDefaultAsync(c => 
                c.Id == chatId && 
                (c.UserId1 == userId || c.UserId2 == userId));

        if (chat != null)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"chat-{chatId}");
        }
    }

    public async Task LeaveChat(int chatId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"chat-{chatId}");
    }

    public async Task MarkMessagesAsRead(int chatId)
    {
        var userId = GetUserId();
        
        var messages = await _db.Messages
            .Where(m => m.ChatId == chatId && m.SenderId != userId && !m.IsRead)
            .ToListAsync();

        foreach (var message in messages)
        {
            message.IsRead = true;
        }

        await _db.SaveChangesAsync();

        await Clients.Group($"chat-{chatId}").SendAsync("MessagesRead", chatId, userId);
    }

    private int GetUserId()
    {
        var claim = Context.User?.FindFirst(ClaimTypes.NameIdentifier);
        return int.TryParse(claim?.Value, out var id) ? id : 0;
    }
}
