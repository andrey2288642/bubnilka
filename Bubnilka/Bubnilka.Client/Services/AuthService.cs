using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Blazored.LocalStorage;
using Bubnilka.Shared.DTOs;

namespace Bubnilka.Client.Services;

public class AuthService
{
    private readonly HttpClient _http;
    private readonly ILocalStorageService _localStorage;
    private readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

    public AuthService(HttpClient http, ILocalStorageService localStorage)
    {
        _http = http;
        _localStorage = localStorage;
    }

    public async Task<AuthResponse> Register(string displayName, string email, string password)
    {
        var request = new RegisterRequest
        {
            DisplayName = displayName,
            Email = email,
            Password = password
        };

        var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
        var response = await _http.PostAsync("api/auth/register", content);
        var result = await response.Content.ReadFromJsonAsync<AuthResponse>(_jsonOptions);

        if (result != null && result.Success && !string.IsNullOrEmpty(result.Token))
        {
            await _localStorage.SetItemAsync("authToken", result.Token);
            await _localStorage.SetItemAsync("currentUser", result.User);
            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", result.Token);
        }

        return result ?? new AuthResponse { Success = false, Error = "Ошибка регистрации" };
    }

    public async Task<AuthResponse> Login(string email, string password)
    {
        var request = new LoginRequest
        {
            Email = email,
            Password = password
        };

        var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
        var response = await _http.PostAsync("api/auth/login", content);
        var result = await response.Content.ReadFromJsonAsync<AuthResponse>(_jsonOptions);

        if (result != null && result.Success && !string.IsNullOrEmpty(result.Token))
        {
            await _localStorage.SetItemAsync("authToken", result.Token);
            await _localStorage.SetItemAsync("currentUser", result.User);
            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", result.Token);
        }

        return result ?? new AuthResponse { Success = false, Error = "Ошибка входа" };
    }

    public async Task Logout()
    {
        await _localStorage.RemoveItemAsync("authToken");
        await _localStorage.RemoveItemAsync("currentUser");
        _http.DefaultRequestHeaders.Authorization = null;
    }

    public async Task<UserInfo?> GetCurrentUser()
    {
        return await _localStorage.GetItemAsync<UserInfo>("currentUser");
    }

    public async Task<string?> GetToken()
    {
        return await _localStorage.GetItemAsync<string>("authToken");
    }

    public async Task<bool> IsAuthenticated()
    {
        var token = await GetToken();
        return !string.IsNullOrEmpty(token);
    }
}
