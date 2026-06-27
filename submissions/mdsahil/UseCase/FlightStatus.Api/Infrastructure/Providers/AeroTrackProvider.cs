using FlightStatus.Api.Domain.Constants;
using FlightStatus.Api.Domain.Entities;
using FlightStatus.Api.Domain.Interfaces;

namespace FlightStatus.Api.Infrastructure.Providers;

/// <summary>
/// Mock provider simulating the 'AeroTrack' external API.
/// AeroTrack typically provides highly detailed flight statuses including exact gate, terminal, and delay reasons.
/// This implementation returns deterministic stub data for specific flight numbers (AI101-AI107) to test the merge logic rules.
/// </summary>
public class AeroTrackProvider : IFlightStatusProvider
{
    // A fixed baseline date used strictly for generating deterministic LastUpdatedUtc timestamps for testing
    private static readonly DateTime ProviderLastUpdatedBase =
    new(2026, 06, 26, 0, 0, 0);
    
    /// <summary>
    /// The unique name identifying this specific provider.
    /// </summary>
    public string ProviderName => "AeroTrack";

    /// <summary>
    /// Retrieves flight status data from the AeroTrack system.
    /// </summary>
    /// <param name="flightNumber">The flight number to query (e.g., "AI101").</param>
    /// <param name="date">The requested flight date.</param>
    /// <returns>A <see cref="ProviderFlightStatus"/> if the flight is found; otherwise, null.</returns>
    public async Task<ProviderFlightStatus?> GetFlightStatusAsync(string flightNumber, DateTime date)
    {
        // Simulate real-world HTTP network latency
        await Task.Delay(50); 

        // Use a C# switch expression to return specific test scenarios based on the flight number
        return flightNumber switch
        {
            // Scenario: Both respond, AeroTrack has latest timestamp (wins)
            "AI101" => CreateStatus(
                "AI101",
                "Departed",
                date,
                ProviderLastUpdatedBase.AddHours(10).AddMinutes(30)),

            // Scenario: Both respond, AeroTrack has older timestamp (loses)
            "AI102" => CreateStatus(
                "AI102",
                "DepartedLate",
                date,
                ProviderLastUpdatedBase.AddHours(10).AddMinutes(10)),

            // Scenario: Cancelled flight
            "AI103" => CreateStatus(
                "AI103",
                "Cancelled",
                date,
                ProviderLastUpdatedBase.AddHours(9).AddMinutes(30)),

            // Scenario: Diverted flight
            "AI104" => CreateStatus(
                "AI104",
                "Diverted",
                date,
                ProviderLastUpdatedBase.AddHours(11)),

            // Scenario: Only AeroTrack responds (wins by default)
            "AI105" => CreateStatus(
                "AI105",
                "Scheduled",
                date,
                ProviderLastUpdatedBase.AddHours(12)),

            // Scenario: AeroTrack doesn't respond
            "AI106" => null,

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

        // If the status implies a delay, add the delay duration to the actual time
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
            Terminal: "T1", // AeroTrack provides verbose terminal data
            Gate: "G14",    // AeroTrack provides verbose gate data
            DelayReason: status == "DepartedLate"
                ? "Late incoming aircraft"
                : null,
            LastUpdatedUtc: lastUpdatedUtc);
    }
}