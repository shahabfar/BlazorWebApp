using BlazorWebApp.Client;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddAuthenticationStateDeserialization();

builder.Services.AddScoped<AuthMessageHandler>();
builder.Services.AddHttpClient("RemoteAPI", client =>
{
    client.BaseAddress = new Uri("https://localhost:7006");
}).AddHttpMessageHandler<AuthMessageHandler>();


await builder.Build().RunAsync();
