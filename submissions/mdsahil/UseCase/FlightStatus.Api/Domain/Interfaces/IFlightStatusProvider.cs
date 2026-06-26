using FlightStatus.Api.Domain.Entities;

namespace FlightStatus.Api.Domain.Interfaces;

public interface IFlightStatusProvider
{
    string ProviderName { get; }
    Task<ProviderFlightStatus?> GetFlightStatusAsync(string flightNumber, DateTime date);
}
