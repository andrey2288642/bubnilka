using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Bubnilka.Server.Data;
using Bubnilka.Shared.Models;
using Bubnilka.Shared.DTOs;
using System.Security.Claims;

namespace Bubnilka.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ChatsController : ControllerBase
{
    private readonly AppDbContext _db;

    public ChatsController(AppDbContext db)
    {
        _db = db;
    }

    private int GetCurrentUserId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier);
        return int.Parse(claim?.Value ?? "0");
    }

    [HttpGet]
    public async Task<ActionResult<List<ChatDto>>> GetChats()
    {
        var userId = GetCurrentUserId();
        
        var chats = await _db.Chats
            .Where(c => c.UserId1 == userId || c.UserId2 == userId)
            .Include(c => c.User1)
            .Include(c => c.User2)
            .Include(c => c.Messages.OrderByDescending(m => m.SentAt).Take(1))
            .OrderByDescending(c => c.LastMessageAt)
            .Select(c => new ChatDto
            {
                Id = c.Id,
                OtherUserId = c.UserId1 == userId ? c.UserId2 : c.UserId1,
                OtherUserName = c.UserId1 == userId ? c.User2.DisplayName : c.User1.DisplayName,
                LastMessage = c.Messages.FirstOrDefault() != null ? c.Messages.FirstOrDefault().Content : "",
                LastMessageAt = c.LastMessageAt,
                IsOtherUserOnline = c.UserId1 == userId ? c.User2.IsOnline : c.User1.IsOnline
            })
            .ToListAsync();

        return Ok(chats);
    }

    [HttpPost]
    public async Task<ActionResult<Chat>> CreateChat([FromBody] CreateChatRequest request)
    {
        var userId = GetCurrentUserId();
        var otherUserId = request.OtherUserId;

        if (userId == otherUserId)
        {
            return BadRequest("Нельзя создать чат с самим собой");
        }

        // Check if chat already exists
        var existingChat = await _db.Chats
            .FirstOrDefaultAsync(c => 
                (c.UserId1 == userId && c.UserId2 == otherUserId) ||
                (c.UserId1 == otherUserId && c.UserId2 == userId));

        if (existingChat != null)
        {
            return Ok(existingChat);
        }

        var chat = new Chat
        {
            UserId1 = userId,
            UserId2 = otherUserId,
            CreatedAt = DateTime.UtcNow
        };

        _db.Chats.Add(chat);
        await _db.SaveChangesAsync();

        return Ok(chat);
    }

    [HttpGet("{chatId}/messages")]
    public async Task<ActionResult<List<Message>>> GetMessages(int chatId)
    {
        var userId = GetCurrentUserId();
        
        var chat = await _db.Chats
            .FirstOrDefaultAsync(c => 
                c.Id == chatId && 
                (c.UserId1 == userId || c.UserId2 == userId));

        if (chat == null)
        {
            return NotFound();
        }

        var messages = await _db.Messages
            .Where(m => m.ChatId == chatId)
            .Include(m => m.Sender)
            .OrderBy(m => m.SentAt)
            .ToListAsync();

        return Ok(messages);
    }

    [HttpGet("users")]
    public async Task<ActionResult<List<UserInfo>>> GetAvailableUsers()
    {
        var userId = GetCurrentUserId();
        
        var users = await _db.Users
            .Where(u => u.Id != userId)
            .Select(u => new UserInfo
            {
                Id = u.Id,
                Email = u.Email,
                DisplayName = u.DisplayName
            })
            .ToListAsync();

        return Ok(users);
    }
}

public class ChatDto
{
    public int Id { get; set; }
    public int OtherUserId { get; set; }
    public string OtherUserName { get; set; } = string.Empty;
    public string LastMessage { get; set; } = string.Empty;
    public DateTime? LastMessageAt { get; set; }
    public bool IsOtherUserOnline { get; set; }
}
