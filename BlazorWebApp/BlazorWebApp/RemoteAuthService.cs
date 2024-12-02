using Microsoft.JSInterop;
using System.Net.Http.Headers;

namespace BlazorWebApp;

public class RemoteAuthService(IHttpClientFactory httpClientFactory, IJSRuntime jsRuntime)
{
    public async Task<string> LoginAsync(string username, string password)
    {
        var client = httpClientFactory.CreateClient("RemoteAPI");
        var response = await client.PostAsJsonAsync("api/auth/login", new { username, password });
        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<LoginResult>();
            //await StoreTokenAsync(result.Token);
            return result.Token;
        }
        return null;
    }

    public async Task<UserInfo> GetUserInfoAsync(string token)
    {
        var client = httpClientFactory.CreateClient("RemoteAPI");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var response = await client.GetAsync("api/auth/userinfo");
        if (response.IsSuccessStatusCode)
            return await response.Content.ReadFromJsonAsync<UserInfo>();

        return null;
    }

    public async Task StoreTokenAsync(string token)
    {
        await jsRuntime.InvokeVoidAsync("localStorage.setItem", "authToken", token);
    }

    public async Task RemoveStoredTokenAsync()
    {
        await jsRuntime.InvokeVoidAsync("localStorage.removeItem", "authToken");
    }

    public async Task<string> GetStoredTokenAsync()
    {
        return await jsRuntime.InvokeAsync<string>("localStorage.getItem", "authToken");
    }

    public async Task<bool> ValidateTokenAsync(string token)
    {
        var client = httpClientFactory.CreateClient("RemoteAPI");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var response = await client.GetAsync("api/auth/validate");
        return response.IsSuccessStatusCode;
    }
}


public class LoginResult
{
    public string Token { get; set; }
}

public class UserInfo
{
    public string Username { get; set; }
    public string Email { get; set; }
}
