using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "your-issuer",
            ValidAudience = "your-audience",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("9GPmKcL7XbHw2kDlKyoaLgA8bRpNhvL3"))
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

//app.MapPost("/api/auth/login", () => "POST request received!");

app.MapPost("/api/auth/login", (LoginRequest request) =>
{
    // Simple hard-coded validation
    if (request.Username == "test" && request.Password == "password")
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, request.Username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Email, "test@example.com") // Add email to claims
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("9GPmKcL7XbHw2kDlKyoaLgA8bRpNhvL3"));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: "your-issuer",
            audience: "your-audience",
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(30),
            signingCredentials: creds);

        return Results.Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
    }

    return Results.Unauthorized();
});

app.MapGet("/api/auth/userinfo", (ClaimsPrincipal user) =>
{
    if (user.Identity?.IsAuthenticated == true)
    {
        var username = user.FindFirstValue(ClaimTypes.NameIdentifier);
        var email = user.FindFirst(ClaimTypes.Email)?.Value;

        var userInfo = new UserInfo(username, email);
        return Results.Ok(userInfo);
    }

    return Results.Unauthorized();
});

app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

record LoginRequest(string Username, string Password);

record UserInfo(string Username, string Email);

