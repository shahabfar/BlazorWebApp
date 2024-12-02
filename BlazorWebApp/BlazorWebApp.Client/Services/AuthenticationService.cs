using Microsoft.JSInterop;
using System.Net.Http.Headers;

namespace BlazorWebApp.Client.Services;

public class AuthenticationService(HttpClient httpClient, IJSRuntime jsRuntime)
{

    public async Task<string> GetTokenAsync()
    {
        return await jsRuntime.InvokeAsync<string>("localStorage.getItem", "authToken");
    }

    public async Task AddAuthHeader(HttpRequestMessage request)
    {
        var token = await GetTokenAsync();
        if (!string.IsNullOrEmpty(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
    }
}
