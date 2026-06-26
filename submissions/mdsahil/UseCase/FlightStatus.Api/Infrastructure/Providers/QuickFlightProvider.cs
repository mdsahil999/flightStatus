using FlightStatus.Api.Domain.Entities;
using FlightStatus.Api.Domain.Interfaces;

namespace FlightStatus.Api.Infrastructure.Providers;

public class QuickFlightProvider : IFlightStatusProvider
{
    public string ProviderName => "QuickFlight";

    public async Task<ProviderFlightStatus?> GetFlightStatusAsync(string flightNumber, DateTime date)
    {
        await Task.Delay(50);

        if (flightNumber == "QF123")
        {
            return new ProviderFlightStatus(
                FlightNumber: flightNumber,
                Status: "Delayed",
                ScheduledDeparture: date.AddHours(10),
                ActualDeparture: date.AddHours(10).AddMinutes(45),
                ScheduledArrival: date.AddHours(14),
                ActualArrival: null,
                Terminal: null,
                Gate: null,
                DelayReason: "Weather",
                LastUpdatedUtc: DateTime.UtcNow.AddMinutes(-1)
            );
        }

        return new ProviderFlightStatus(
            FlightNumber: flightNumber,
            Status: "Scheduled",
            ScheduledDeparture: date.AddHours(10),
            ActualDeparture: null,
            ScheduledArrival: date.AddHours(14),
            ActualArrival: null,
            Terminal: null,
            Gate: null,
            DelayReason: null,
            LastUpdatedUtc: DateTime.UtcNow.AddMinutes(-15)
        );
    }
}
