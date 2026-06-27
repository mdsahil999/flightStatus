using FlightStatus.Api.Domain.Constants;
using FlightStatus.Api.Domain.Entities;
using FlightStatus.Api.Domain.Interfaces;

namespace FlightStatus.Api.Infrastructure.Providers;

public class QuickFlightProvider : IFlightStatusProvider
{
    private static readonly DateTime ProviderLastUpdatedBase =
    new(2026, 06, 26, 0, 0, 0);
    public string ProviderName => "QuickFlight";

    public async Task<ProviderFlightStatus?> GetFlightStatusAsync(string flightNumber, DateTime date)
    {
        await Task.Delay(30); // Simulate network latency

        return flightNumber switch
        {
            "AI101" => CreateStatus(
                "AI101",
                "RunningBehind",
                date,
                ProviderLastUpdatedBase.AddHours(10).AddMinutes(15)),

            "AI102" => CreateStatus(
                "AI102",
                "OnSchedule",
                date,
                ProviderLastUpdatedBase.AddHours(10).AddMinutes(45)),

            "AI103" => CreateStatus(
                "AI103",
                "NotOperating",
                date,
                ProviderLastUpdatedBase.AddHours(9).AddMinutes(20)),

            "AI104" => CreateStatus(
                "AI104",
                "LandedElsewhere",
                date,
                ProviderLastUpdatedBase.AddHours(11).AddMinutes(15)),

            "AI105" => null,

            "AI106" => CreateStatus(
                "AI106",
                "RunningBehind",
                date,
                ProviderLastUpdatedBase.AddHours(12)),

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

        var actualDeparture = status == "RunningBehind"
            ? scheduledDeparture + FlightConstants.DelayDuration
            : scheduledDeparture;

        var scheduledArrival = scheduledDeparture + FlightConstants.FlightDuration;

        var actualArrival = status == "RunningBehind"
            ? scheduledArrival + FlightConstants.DelayDuration
            : scheduledArrival;

        return new ProviderFlightStatus(
            FlightNumber: flightNumber,
            Status: status,
            ScheduledDeparture: scheduledDeparture,
            ActualDeparture: actualDeparture,
            ScheduledArrival: scheduledArrival,
            ActualArrival: actualArrival,
            Terminal: null,
            Gate: null,
            DelayReason: null,
            LastUpdatedUtc: lastUpdatedUtc);
    }
}