using System;
using System.Threading.Tasks;
using Xunit;
using Moq;
using FlightStatus.Api.Application.Services;
using FlightStatus.Api.Domain.Entities;
using FlightStatus.Api.Domain.Enums;
using FlightStatus.Api.Domain.Interfaces;
using FlightStatus.Api.Infrastructure.Repositories;
using Microsoft.Extensions.Logging;

namespace FlightStatus.Tests;

public class ScenarioTests
{
    private readonly Mock<IFlightStatusProvider> _aeroTrackMock;
    private readonly Mock<IFlightStatusProvider> _quickFlightMock;
    private readonly FlightStatusService _service;

    public ScenarioTests()
    {
        _aeroTrackMock = new Mock<IFlightStatusProvider>();
        _aeroTrackMock.SetupGet(p => p.ProviderName).Returns("AeroTrack");

        _quickFlightMock = new Mock<IFlightStatusProvider>();
        _quickFlightMock.SetupGet(p => p.ProviderName).Returns("QuickFlight");
        
        var mergeService = new MergeService();
        var normalizer = new StatusNormalizer();
        
        var repo = new FlightStatusRepository(
            new IFlightStatusProvider[] { _aeroTrackMock.Object, _quickFlightMock.Object }, 
            mergeService, 
            new Mock<ILogger<FlightStatusRepository>>().Object);
            
        _service = new FlightStatusService(repo, normalizer);
    }

    private ProviderFlightStatus CreateMockStatus(string provider, string status, DateTime lastUpdated)
    {
        return new ProviderFlightStatus(
            FlightNumber: "TEST",
            Status: status,
            ScheduledDeparture: DateTime.UtcNow,
            ActualDeparture: null,
            ScheduledArrival: DateTime.UtcNow,
            ActualArrival: null,
            Terminal: provider == "AeroTrack" ? "T1" : null,
            Gate: null,
            DelayReason: null,
            LastUpdatedUtc: lastUpdated
        );
    }

    private void SetupProviders(ProviderFlightStatus? aeroTrackResult, ProviderFlightStatus? quickFlightResult)
    {
        _aeroTrackMock.Setup(p => p.GetFlightStatusAsync(It.IsAny<string>(), It.IsAny<DateTime>()))
                      .ReturnsAsync(aeroTrackResult);
                      
        _quickFlightMock.Setup(p => p.GetFlightStatusAsync(It.IsAny<string>(), It.IsAny<DateTime>()))
                        .ReturnsAsync(quickFlightResult);
    }

    [Fact]
    public async Task Scenario_AI101_AeroTrack_Departed_QuickFlight_RunningBehind()
    {
        var aero = CreateMockStatus("AeroTrack", "Departed", DateTime.UtcNow.AddMinutes(30)); // 10:30
        var quick = CreateMockStatus("QuickFlight", "RunningBehind", DateTime.UtcNow.AddMinutes(15)); // 10:15
        SetupProviders(aero, quick);

        var result = await _service.GetFlightStatusAsync("AI101", DateTime.UtcNow);

        Assert.Equal(FlightStatusEnum.OnTime, result.Status); // AeroTrack wins (Departed -> OnTime)
    }

    [Fact]
    public async Task Scenario_AI102_AeroTrack_DepartedLate_QuickFlight_OnSchedule()
    {
        var aero = CreateMockStatus("AeroTrack", "DepartedLate", DateTime.UtcNow.AddMinutes(10)); // 10:10
        var quick = CreateMockStatus("QuickFlight", "OnSchedule", DateTime.UtcNow.AddMinutes(45)); // 10:45
        SetupProviders(aero, quick);

        var result = await _service.GetFlightStatusAsync("AI102", DateTime.UtcNow);

        Assert.Equal(FlightStatusEnum.OnTime, result.Status); // QuickFlight wins (OnSchedule -> OnTime)
    }

    [Fact]
    public async Task Scenario_AI103_AeroTrack_Cancelled_QuickFlight_NotOperating()
    {
        var aero = CreateMockStatus("AeroTrack", "Cancelled", DateTime.UtcNow.AddMinutes(30)); // 09:30
        var quick = CreateMockStatus("QuickFlight", "NotOperating", DateTime.UtcNow.AddMinutes(20)); // 09:20
        SetupProviders(aero, quick);

        var result = await _service.GetFlightStatusAsync("AI103", DateTime.UtcNow);

        Assert.Equal(FlightStatusEnum.Cancelled, result.Status); // AeroTrack wins
    }

    [Fact]
    public async Task Scenario_AI104_AeroTrack_Diverted_QuickFlight_LandedElsewhere()
    {
        var aero = CreateMockStatus("AeroTrack", "Diverted", DateTime.UtcNow.AddMinutes(0)); // 11:00
        var quick = CreateMockStatus("QuickFlight", "LandedElsewhere", DateTime.UtcNow.AddMinutes(15)); // 11:15
        SetupProviders(aero, quick);

        var result = await _service.GetFlightStatusAsync("AI104", DateTime.UtcNow);

        Assert.Equal(FlightStatusEnum.Diverted, result.Status); // QuickFlight wins
    }

    [Fact]
    public async Task Scenario_AI105_AeroTrack_Scheduled_QuickFlight_NoResponse()
    {
        var aero = CreateMockStatus("AeroTrack", "Scheduled", DateTime.UtcNow.AddMinutes(0)); // 12:00
        SetupProviders(aero, null);

        var result = await _service.GetFlightStatusAsync("AI105", DateTime.UtcNow);

        Assert.Equal(FlightStatusEnum.OnTime, result.Status); // AeroTrack wins fallback
    }

    [Fact]
    public async Task Scenario_AI106_AeroTrack_NoResponse_QuickFlight_RunningBehind()
    {
        var quick = CreateMockStatus("QuickFlight", "RunningBehind", DateTime.UtcNow.AddMinutes(0)); // 12:00
        SetupProviders(null, quick);

        var result = await _service.GetFlightStatusAsync("AI106", DateTime.UtcNow);

        Assert.Equal(FlightStatusEnum.Delayed, result.Status); // QuickFlight wins fallback
    }

    [Fact]
    public async Task Scenario_AI107_Both_NoResponse()
    {
        SetupProviders(null, null);

        var result = await _service.GetFlightStatusAsync("AI107", DateTime.UtcNow);

        Assert.Equal(FlightStatusEnum.Unknown, result.Status); // Empty fallback
    }
}
