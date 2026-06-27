using FlightStatus.Api.Domain.Entities;
using FlightStatus.Api.Application.Services;

namespace FlightStatus.Tests;

public class MergeServiceTests
{
    private readonly MergeService _service = new();

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

    [Fact]
    public void Merge_Rule1_BothRespond_PrefersLatestTimestamp()
    {
        // Scenario AI101: AeroTrack (10:30) vs QuickFlight (10:15)
        var aero = CreateMockStatus("AeroTrack", "Departed", DateTime.UtcNow.AddMinutes(30));
        var quick = CreateMockStatus("QuickFlight", "RunningBehind", DateTime.UtcNow.AddMinutes(15));

        var result = _service.Merge(new[] { aero, quick });

        Assert.NotNull(result);
        Assert.Equal("Departed", result.Status); // The raw string returned by AeroTrack
        Assert.Equal("T1", result.Terminal); // Proves it picked AeroTrack
    }

    [Fact]
    public void Merge_Rule1_BothRespond_PrefersLatestTimestamp_Reverse()
    {
        // Scenario AI102: AeroTrack (10:10) vs QuickFlight (10:45)
        var aero = CreateMockStatus("AeroTrack", "DepartedLate", DateTime.UtcNow.AddMinutes(10));
        var quick = CreateMockStatus("QuickFlight", "OnSchedule", DateTime.UtcNow.AddMinutes(45));

        var result = _service.Merge(new[] { aero, quick });

        Assert.NotNull(result);
        Assert.Equal("OnSchedule", result.Status); // The raw string returned by QuickFlight
        Assert.Null(result.Terminal); // Proves it picked QuickFlight
    }

    [Fact]
    public void Merge_Rule2_OnlyOneResponds_UsesThatResult()
    {
        // Scenario AI105: AeroTrack responds, QuickFlight null
        var aero = CreateMockStatus("AeroTrack", "Scheduled", DateTime.UtcNow);

        var result = _service.Merge(new[] { aero, null });

        Assert.NotNull(result);
        Assert.Equal("Scheduled", result.Status);
        Assert.Equal("T1", result.Terminal);
    }

    [Fact]
    public void Merge_Rule3_NeitherResponds_ReturnsNull()
    {
        var result = _service.Merge(new ProviderFlightStatus?[] { null, null });

        Assert.Null(result);
    }
}
