using FlightStatus.Api.Application.Services;
using FlightStatus.Api.Domain.Interfaces;
using FlightStatus.Api.Infrastructure.Providers;
using FlightStatus.Api.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});

// DI Registration
builder.Services.AddSingleton<StatusNormalizer>();
builder.Services.AddSingleton<MergeService>();
builder.Services.AddScoped<IFlightStatusRepository, FlightStatusRepository>();
builder.Services.AddScoped<FlightStatusService>();
builder.Services.AddScoped<IFlightStatusProvider, AeroTrackProvider>();
builder.Services.AddScoped<IFlightStatusProvider, QuickFlightProvider>();

var app = builder.Build();

app.UseCors();
// app.UseHttpsRedirection(); // Disabled for local development to prevent untrusted cert issues

// Minimal API Endpoint
app.MapGet("/flights/status", async (
    [FromQuery] string? flightNumber, 
    [FromQuery] DateTime? date, 
    FlightStatusService service) =>
{
    if (string.IsNullOrWhiteSpace(flightNumber) || !date.HasValue)
    {
        return Results.BadRequest(new { Error = "flightNumber and date are required parameters." });
    }

    var result = await service.GetFlightStatusAsync(flightNumber, date.Value);
    return Results.Ok(result);
})
.WithName("GetFlightStatus")
.WithOpenApi();

app.Run();
