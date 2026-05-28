using Microsoft.AspNetCore.SignalR.Client;
using Blazored.LocalStorage;

namespace Bubnilka.Client.Services;

public class SignalRService : IAsyncDisposable
{
    private HubConnection? _hubConnection;
    private readonly AuthService _auth;
    
    public event Func<int, bool, Task>? OnUserStatusChanged;
    public event Func<dynamic, Task>? OnMessageReceived;

    public SignalRService(AuthService auth)
    {
        _auth = auth;
    }

    public async Task Initialize()
    {
        if (_hubConnection != null) return;

        var token = await _auth.GetToken();
        if (string.IsNullOrEmpty(token)) return;

        _hubConnection = new HubConnectionBuilder()
            .WithUrl("https://localhost:7001/chathub", options =>
            {
                options.AccessTokenProvider = () => Task.FromResult(token);
            })
            .WithAutomaticReconnect()
            .Build();

        _hubConnection.On<int, bool>("UserStatusChanged", async (userId, isOnline) =>
        {
            if (OnUserStatusChanged != null)
            {
                await OnUserStatusChanged.Invoke(userId, isOnline);
            }
        });

        _hubConnection.On<dynamic>("ReceiveMessage", async (message) =>
        {
            if (OnMessageReceived != null)
            {
                await OnMessageReceived.Invoke(message);
            }
        });

        try
        {
            await _hubConnection.StartAsync();
        }
        catch
        {
            // Connection failed - will retry
        }
    }

    public async Task JoinChat(int chatId)
    {
        if (_hubConnection == null) return;
        
        try
        {
            await _hubConnection.InvokeAsync("JoinChat", chatId);
        }
        catch { }
    }

    public async Task LeaveChat(int chatId)
    {
        if (_hubConnection == null) return;
        
        try
        {
            await _hubConnection.InvokeAsync("LeaveChat", chatId);
        }
        catch { }
    }

    public async Task SendMessage(int chatId, string content)
    {
        if (_hubConnection == null) return;
        
        try
        {
            await _hubConnection.InvokeAsync("SendMessage", chatId, content);
        }
        catch { }
    }

    public async ValueTask DisposeAsync()
    {
        if (_hubConnection != null)
        {
            await _hubConnection.DisposeAsync();
        }
    }
}
