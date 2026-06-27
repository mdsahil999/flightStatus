using FlightStatus.Api.Domain.Constants;
using FlightStatus.Api.Domain.Entities;
using FlightStatus.Api.Domain.Interfaces;

namespace FlightStatus.Api.Infrastructure.Providers;

public class AeroTrackProvider : IFlightStatusProvider
{
    private static readonly DateTime ProviderLastUpdatedBase =
    new(2026, 06, 26, 0, 0, 0);
    public string ProviderName => "AeroTrack";

    public async Task<ProviderFlightStatus?> GetFlightStatusAsync(string flightNumber, DateTime date)
    {
        await Task.Delay(50); // Simulate network latency

        var lastUpdatedBase = date.Date;

        return flightNumber switch
        {
            "AI101" => CreateStatus(
                "AI101",
                "Departed",
                date,
                ProviderLastUpdatedBase.AddHours(10).AddMinutes(30)),

            "AI102" => CreateStatus(
                "AI102",
                "DepartedLate",
                date,
                ProviderLastUpdatedBase.AddHours(10).AddMinutes(10)),

            "AI103" => CreateStatus(
                "AI103",
                "Cancelled",
                date,
                ProviderLastUpdatedBase.AddHours(9).AddMinutes(30)),

            "AI104" => CreateStatus(
                "AI104",
                "Diverted",
                date,
                ProviderLastUpdatedBase.AddHours(11)),

            "AI105" => CreateStatus(
                "AI105",
                "Scheduled",
                date,
                ProviderLastUpdatedBase.AddHours(12)),

            "AI106" => null,

            "AI107" => null,

            _ => null
        };
    }

    private ProviderFlightStatus CreateStatus(
        string flightNumber,
        string status,
        DateTime flightDate,
        DateTime lastUpdatedUtc)
    {
        var scheduledDeparture = flightDate.Date + FlightConstants.ScheduledDepartureTime;

        var actualDeparture = status == "DepartedLate"
            ? scheduledDeparture + FlightConstants.DelayDuration
            : scheduledDeparture;

        var scheduledArrival = scheduledDeparture + FlightConstants.FlightDuration;

        var actualArrival = status == "DepartedLate"
            ? scheduledArrival + FlightConstants.DelayDuration
            : scheduledArrival;

        return new ProviderFlightStatus(
            FlightNumber: flightNumber,
            Status: status,
            ScheduledDeparture: scheduledDeparture,
            ActualDeparture: actualDeparture,
            ScheduledArrival: scheduledArrival,
            ActualArrival: actualArrival,
            Terminal: "T1",
            Gate: "G14",
            DelayReason: status == "DepartedLate"
                ? "Late incoming aircraft"
                : null,
            LastUpdatedUtc: lastUpdatedUtc);
    }
}