using FlightStatus.Api.Domain.Constants;
using FlightStatus.Api.Domain.Entities;
using FlightStatus.Api.Domain.Interfaces;

namespace FlightStatus.Api.Infrastructure.Providers;

/// <summary>
/// Mock provider simulating the 'QuickFlight' external API.
/// QuickFlight is typically a faster, more lightweight provider, but it provides less detailed information
/// compared to AeroTrack. For instance, it does NOT provide Terminal, Gate, or explicit DelayReason data.
/// This implementation returns deterministic stub data for specific flight numbers (AI101-AI107) to test merge logic.
/// </summary>
public class QuickFlightProvider : IFlightStatusProvider
{
    // A fixed baseline date used strictly for generating deterministic LastUpdatedUtc timestamps for testing
    private static readonly DateTime ProviderLastUpdatedBase =
    new(2026, 06, 26, 0, 0, 0);
    
    /// <summary>
    /// The unique name identifying this specific provider.
    /// </summary>
    public string ProviderName => "QuickFlight";

    /// <summary>
    /// Retrieves flight status data from the QuickFlight system.
    /// </summary>
    /// <param name="flightNumber">The flight number to query (e.g., "AI101").</param>
    /// <param name="date">The requested flight date.</param>
    /// <returns>A <see cref="ProviderFlightStatus"/> if the flight is found; otherwise, null.</returns>
    public async Task<ProviderFlightStatus?> GetFlightStatusAsync(string flightNumber, DateTime date)
    {
        // Simulate real-world HTTP network latency (slightly faster than AeroTrack)
        await Task.Delay(30); 

        // Use a C# switch expression to return specific test scenarios based on the flight number
        return flightNumber switch
        {
            // Scenario: Both respond, QuickFlight has older timestamp (10:15) than AeroTrack (10:30). QuickFlight loses.
            "AI101" => CreateStatus(
                "AI101",
                "RunningBehind",
                date,
                ProviderLastUpdatedBase.AddHours(10).AddMinutes(15)),

            // Scenario: Both respond, QuickFlight has newer timestamp (10:45) than AeroTrack (10:10). QuickFlight wins.
            "AI102" => CreateStatus(
                "AI102",
                "OnSchedule",
                date,
                ProviderLastUpdatedBase.AddHours(10).AddMinutes(45)),

            // Scenario: Cancelled flight ("NotOperating" in QuickFlight vocabulary)
            "AI103" => CreateStatus(
                "AI103",
                "NotOperating",
                date,
                ProviderLastUpdatedBase.AddHours(9).AddMinutes(20)),

            // Scenario: Diverted flight ("LandedElsewhere" in QuickFlight vocabulary)
            "AI104" => CreateStatus(
                "AI104",
                "LandedElsewhere",
                date,
                ProviderLastUpdatedBase.AddHours(11).AddMinutes(15)),

            // Scenario: QuickFlight doesn't respond (only AeroTrack responds)
            "AI105" => null,

            // Scenario: Only QuickFlight responds (wins by default)
            "AI106" => CreateStatus(
                "AI106",
                "RunningBehind",
                date,
                ProviderLastUpdatedBase.AddHours(12)),

            // Scenario: Neither provider responds
            "AI107" => null,

            _ => null
        };
    }

    /// <summary>
    /// Helper method to construct a standard ProviderFlightStatus response.
    /// </summary>
    private ProviderFlightStatus CreateStatus(
        string flightNumber,
        string status,
        DateTime flightDate,
        DateTime lastUpdatedUtc)
    {
        // Calculate the base scheduled time using the global constants
        var scheduledDeparture = flightDate.Date + FlightConstants.ScheduledDepartureTime;

        // If the status implies a delay ("RunningBehind"), add the delay duration to the actual time
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
            // Note: QuickFlight does NOT return terminal, gate, or delay reason
            Terminal: null,
            Gate: null,
            DelayReason: null,
            LastUpdatedUtc: lastUpdatedUtc);
    }
}