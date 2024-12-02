using BlazorWebApp.Client.Services;

namespace BlazorWebApp.Client;

public class AuthMessageHandler(AuthenticationService authService) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        await authService.AddAuthHeader(request);
        return await base.SendAsync(request, cancellationToken);
    }
}
