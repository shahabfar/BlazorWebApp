using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace BlazorWebApp;

public class CustomAuthenticationStateProvider(RemoteAuthService authService, IHttpContextAccessor httpContextAccessor) : AuthenticationStateProvider
{
    private bool _isInteractive;
    private ClaimsPrincipal _anonymous => new(new ClaimsIdentity());

    public void SetInteractivity(bool isInteractive)
    {
        _isInteractive = isInteractive;
    }

    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var httpContext = httpContextAccessor.HttpContext;
        var authToken = httpContext?.Request.Cookies["AuthToken"];

        if (string.IsNullOrEmpty(authToken))
        {
            return Task.FromResult(new AuthenticationState(_anonymous));
        }

        // Validate token and create claims
        var claims = new List<Claim>
        {
            // Populate claims from token
        };

        var identity = new ClaimsIdentity(claims, "jwt");
        var user = new ClaimsPrincipal(identity);

        return Task.FromResult(new AuthenticationState(user));
    }

    public async Task MarkUserAsAuthenticated(string token)
    {
        //await authService.StoreTokenAsync(token);

        var userInfo = await authService.GetUserInfoAsync(token);
        var identity = new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.Name, userInfo.Username),
            new Claim(ClaimTypes.Email, userInfo.Email),
            new Claim("JWT", token)
        }, "JwtAuth");

        var user = new ClaimsPrincipal(identity);
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
    }

    public async Task MarkUserAsLoggedOut()
    {
        await authService.RemoveStoredTokenAsync();
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_anonymous)));
    }

}
