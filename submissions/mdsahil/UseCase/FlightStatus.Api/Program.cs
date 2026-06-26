using FlightStatus.Api.Application.Services;
using FlightStatus.Api.Domain.Interfaces;
using FlightStatus.Api.Infrastructure.Providers;
using FlightStatus.Api.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
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

app.MapControllers();

app.Run();
