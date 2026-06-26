using FlightStatus.Api.Domain.Entities;
using FlightStatus.Api.Application.Services;

namespace FlightStatus.Tests;

public class MergeServiceTests
{
    private readonly MergeService _service = new();

    [Fact]
    public void Merge_ReturnsLatest_BasedOnLastUpdatedUtc()
    {
        var oldDate = DateTime.UtcNow.AddMinutes(-10);
        var newDate = DateTime.UtcNow;

        var older = new ProviderFlightStatus("QF1", "Delayed", DateTime.UtcNow, null, DateTime.UtcNow, null, null, null, null, oldDate);
        var newer = new ProviderFlightStatus("QF1", "On Time", DateTime.UtcNow, null, DateTime.UtcNow, null, null, null, null, newDate);

        var result = _service.Merge(new[] { older, newer });

        Assert.NotNull(result);
        Assert.Equal("On Time", result.Status);
    }

    [Fact]
    public void Merge_IgnoresNulls()
    {
        var valid = new ProviderFlightStatus("QF1", "Delayed", DateTime.UtcNow, null, DateTime.UtcNow, null, null, null, null, DateTime.UtcNow);
        var result = _service.Merge(new[] { null, valid });
        
        Assert.NotNull(result);
        Assert.Equal("Delayed", result.Status);
    }
}
