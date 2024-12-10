using IP2C_consumer.Database;
using IP2C_consumer.Models;
using IP2C_consumer.Services;
using IP2C_consumer.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

// Add Redis
builder.Services.AddSingleton<IConnectionMultiplexer>(_ =>
    ConnectionMultiplexer.Connect(builder.Configuration["Redis:ConnectionString"]));

// Add HttpClient
builder.Services.AddHttpClient<IIP2CService, IP2CService>();

// Add other services
builder.Services.AddScoped<ICacheService, RedisCacheService>();
builder.Services.AddScoped<IIPDetailsService, IPDetailsService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Minimal API endpoint
app.MapGet("/api/ip/{ipAddress}", async (string ipAddress, IIPDetailsService ipDetailsService) =>
{
    /// <summary>
    /// Fail Fast (Early Validation)
    /// Ensures that invalid input is detected as early as possible, preventing unnecessary operations or cascading failures deeper into the system.
    /// </summary>
    if (!ipDetailsService.IsValidIpAddress(ipAddress))
        return Results.BadRequest("Invalid IP address");

    var country = await ipDetailsService.GetIPDetailsAsync(ipAddress);

    if (country == null)
        return Results.NotFound("IP details not found");

    return Results.Ok(new IpDetailsDTO(country.Name, country.TwoLetterCode, country.ThreeLetterCode));
});

app.UseHttpsRedirection();

app.Run();