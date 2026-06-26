using FlightStatus.Api.Domain.Entities;

namespace FlightStatus.Api.Domain.Interfaces;

public interface IFlightStatusRepository
{
    Task<ProviderFlightStatus?> GetFlightDataAsync(string flightNumber, DateTime date);
}
