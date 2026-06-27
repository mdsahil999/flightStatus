using FlightStatus.Api.Application.Services;
using FlightStatus.Api.Domain.Entities;
using FlightStatus.Api.Domain.Interfaces;

namespace FlightStatus.Api.Infrastructure.Repositories;

public class FlightStatusRepository : IFlightStatusRepository
{
    private readonly IEnumerable<IFlightStatusProvider> _providers;
    private readonly MergeService _mergeService;
    private readonly ILogger<FlightStatusRepository> _logger;

    public FlightStatusRepository(
        IEnumerable<IFlightStatusProvider> providers,
        MergeService mergeService,
        ILogger<FlightStatusRepository> logger)
    {
        _providers = providers;
        _mergeService = mergeService;
        _logger = logger;
    }

    public async Task<ProviderFlightStatus?> GetFlightDataAsync(string flightNumber, DateTime date)
    {
        var tasks = _providers.Select(async provider =>
        {
            try
            {
                return await provider.GetFlightStatusAsync(flightNumber, date);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Provider {ProviderName} failed for flight {FlightNumber}", provider.ProviderName, flightNumber);
                return null;
            }
        });

        var results = await Task.WhenAll(tasks);
        
        return _mergeService.Merge(results);
    }
}
