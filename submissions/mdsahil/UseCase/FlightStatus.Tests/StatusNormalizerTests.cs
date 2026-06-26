using FlightStatus.Api.Domain.Enums;
using FlightStatus.Api.Domain.Entities;
using FlightStatus.Api.Application.Services;

namespace FlightStatus.Tests;

public class StatusNormalizerTests
{
    private readonly StatusNormalizer _normalizer = new();

    [Fact]
    public void Normalize_Cancelled_ReturnsCancelled()
    {
        var status = new ProviderFlightStatus("QF1", "Cancelled", DateTime.UtcNow, null, DateTime.UtcNow, null, null, null, null, DateTime.UtcNow);
        Assert.Equal(FlightStatusEnum.Cancelled, _normalizer.Normalize(status));
    }

    [Fact]
    public void Normalize_Diverted_ReturnsDiverted()
    {
        var status = new ProviderFlightStatus("QF1", "Diverted", DateTime.UtcNow, null, DateTime.UtcNow, null, null, null, null, DateTime.UtcNow);
        Assert.Equal(FlightStatusEnum.Diverted, _normalizer.Normalize(status));
    }

    [Fact]
    public void Normalize_Delayed_String_ReturnsDelayed()
    {
        var status = new ProviderFlightStatus("QF1", "Delayed", DateTime.UtcNow, null, DateTime.UtcNow, null, null, null, null, DateTime.UtcNow);
        Assert.Equal(FlightStatusEnum.Delayed, _normalizer.Normalize(status));
    }

    [Fact]
    public void Normalize_ActualDepartureOver15Mins_ReturnsDelayed()
    {
        var scheduled = DateTime.UtcNow;
        var actual = scheduled.AddMinutes(16);
        var status = new ProviderFlightStatus("QF1", "Scheduled", scheduled, actual, DateTime.UtcNow, null, null, null, null, DateTime.UtcNow);
        Assert.Equal(FlightStatusEnum.Delayed, _normalizer.Normalize(status));
    }

    [Fact]
    public void Normalize_ActualDepartureUnder15Mins_ReturnsOnTime()
    {
        var scheduled = DateTime.UtcNow;
        var actual = scheduled.AddMinutes(10);
        var status = new ProviderFlightStatus("QF1", "Scheduled", scheduled, actual, DateTime.UtcNow, null, null, null, null, DateTime.UtcNow);
        Assert.Equal(FlightStatusEnum.OnTime, _normalizer.Normalize(status));
    }

    [Fact]
    public void Normalize_NoActualDeparture_ReturnsOnTime()
    {
        var status = new ProviderFlightStatus("QF1", "Scheduled", DateTime.UtcNow, null, DateTime.UtcNow, null, null, null, null, DateTime.UtcNow);
        Assert.Equal(FlightStatusEnum.OnTime, _normalizer.Normalize(status));
    }
}
