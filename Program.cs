using IP2C_consumer.Database;
using IP2C_consumer.Models;
using IP2C_consumer.Services;
using IP2C_consumer.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddSingleton<IConnectionMultiplexer>(_ =>
    ConnectionMultiplexer.Connect(builder.Configuration["Redis:ConnectionString"]));

builder.Services.AddHttpClient<IIP2CService, IP2CService>();

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

app.MapGet("/api/ip/{ipAddress}", async (string ipAddress, IIPDetailsService ipDetailsService) =>
{
    if (!ipDetailsService.IsValidIpAddress(ipAddress))
        return Results.BadRequest("Invalid IP address");

    var country = await ipDetailsService.GetIPDetailsAsync(ipAddress);

    if (country == null)
        return Results.NotFound("IP details not found");

    return Results.Ok(new IpDetailsDTO(country.Name, country.TwoLetterCode, country.ThreeLetterCode));
});

app.UseHttpsRedirection();

app.Run();