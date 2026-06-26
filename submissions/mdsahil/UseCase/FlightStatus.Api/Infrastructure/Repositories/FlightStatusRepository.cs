using FlightStatus.Api.Application.Services;
using FlightStatus.Api.Domain.Entities;
using FlightStatus.Api.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace FlightStatus.Api.Infrastructure.Repositories;

public class FlightStatusRepository(
    IEnumerable<IFlightStatusProvider> providers,
    MergeService mergeService,
    ILogger<FlightStatusRepository> logger) : IFlightStatusRepository
{
    public async Task<ProviderFlightStatus?> GetFlightDataAsync(string flightNumber, DateTime date)
    {
        var tasks = providers.Select(async provider =>
        {
            try
            {
                return await provider.GetFlightStatusAsync(flightNumber, date);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Provider {ProviderName} failed for flight {FlightNumber}", provider.ProviderName, flightNumber);
                return null;
            }
        });

        var results = await Task.WhenAll(tasks);
        
        return mergeService.Merge(results);
    }
}
