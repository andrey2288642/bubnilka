using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Blazored.LocalStorage;
using Bubnilka.Shared.Models;
using Bubnilka.Shared.DTOs;

namespace Bubnilka.Client.Services;

public class ChatService
{
    private readonly HttpClient _http;
    private readonly AuthService _auth;
    private readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

    public ChatService(HttpClient http, AuthService auth)
    {
        _http = http;
        _auth = auth;
    }

    private async Task EnsureAuthHeader()
    {
        var token = await _auth.GetToken();
        if (!string.IsNullOrEmpty(token))
        {
            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
    }

    public async Task<List<ChatDto>> GetChats()
    {
        await EnsureAuthHeader();
        try
        {
            var response = await _http.GetAsync("api/chats");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<ChatDto>>(_jsonOptions) ?? new List<ChatDto>();
            }
        }
        catch { }
        return new List<ChatDto>();
    }

    public async Task<Chat?> CreateChat(int otherUserId)
    {
        await EnsureAuthHeader();
        var request = new CreateChatRequest { OtherUserId = otherUserId };
        var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
        
        try
        {
            var response = await _http.PostAsync("api/chats", content);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<Chat>(_jsonOptions);
            }
        }
        catch { }
        return null;
    }

    public async Task<List<Message>> GetMessages(int chatId)
    {
        await EnsureAuthHeader();
        try
        {
            var response = await _http.GetAsync($"api/chats/{chatId}/messages");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<Message>>(_jsonOptions) ?? new List<Message>();
            }
        }
        catch { }
        return new List<Message>();
    }

    public async Task<List<UserInfo>> GetAvailableUsers()
    {
        await EnsureAuthHeader();
        try
        {
            var response = await _http.GetAsync("api/chats/users");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<UserInfo>>(_jsonOptions) ?? new List<UserInfo>();
            }
        }
        catch { }
        return new List<UserInfo>();
    }

    public async Task MarkMessagesAsRead(int chatId)
    {
        await EnsureAuthHeader();
        try
        {
            await _http.PostAsync($"api/chats/{chatId}/read", null);
        }
        catch { }
    }
}

public class ChatDto
{
    public int Id { get; set; }
    public int OtherUserId { get; set; }
    public string OtherUserName { get; set; } = "";
    public string LastMessage { get; set; } = "";
    public DateTime? LastMessageAt { get; set; }
    public bool IsOtherUserOnline { get; set; }
}
