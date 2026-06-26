using FlightStatus.Api.Domain.Entities;
using FlightStatus.Api.Domain.Interfaces;

namespace FlightStatus.Api.Infrastructure.Providers;

public class AeroTrackProvider : IFlightStatusProvider
{
    public string ProviderName => "AeroTrack";

    public async Task<ProviderFlightStatus?> GetFlightStatusAsync(string flightNumber, DateTime date)
    {
        await Task.Delay(100);

        if (flightNumber == "TIMEOUT")
        {
            await Task.Delay(5000);
            throw new TimeoutException("AeroTrack timed out.");
        }

        if (flightNumber == "QF123") return null;

        return new ProviderFlightStatus(
            FlightNumber: flightNumber,
            Status: "On Time",
            ScheduledDeparture: date.AddHours(10),
            ActualDeparture: date.AddHours(10),
            ScheduledArrival: date.AddHours(14),
            ActualArrival: null,
            Terminal: "T1",
            Gate: "G14",
            DelayReason: null,
            LastUpdatedUtc: DateTime.UtcNow.AddMinutes(-5)
        );
    }
}
