using FlightStatus.Api.Application.Services;
using FlightStatus.Api.Domain.Entities;
using FlightStatus.Api.Domain.Interfaces;

namespace FlightStatus.Api.Infrastructure.Repositories;

public class FlightStatusRepository : IFlightStatusRepository
{
    private readonly IEnumerable<IFlightStatusProvider> _providers;
    private readonly MergeService _mergeService;
    private readonly ILogger<FlightStatusRepository> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="FlightStatusRepository"/>.
    /// </summary>
    /// <param name="providers">An enumerable collection of all registered flight status providers (e.g., AeroTrack, QuickFlight).</param>
    /// <param name="mergeService">The domain service responsible for applying business rules to merge multiple provider results.</param>
    /// <param name="logger">The logging service for recording provider failures and diagnostic data.</param>
    public FlightStatusRepository(
        IEnumerable<IFlightStatusProvider> providers,
        MergeService mergeService,
        ILogger<FlightStatusRepository> logger)
    {
        _providers = providers;
        _mergeService = mergeService;
        _logger = logger;
    }

    /// <summary>
    /// Queries all available flight status providers concurrently and merges their responses.
    /// </summary>
    /// <param name="flightNumber">The specific flight number to track (e.g., "AI106").</param>
    /// <param name="date">The requested date of the flight.</param>
    /// <returns>A single, consolidated <see cref="ProviderFlightStatus"/> representing the best available data, or null if no provider returned data.</returns>
    public async Task<ProviderFlightStatus?> GetFlightDataAsync(string flightNumber, DateTime date)
    {
        // 1. Scatter Phase: Map each provider into an asynchronous task.
        var tasks = _providers.Select(async provider =>
        {
            try
            {
                // Attempt to fetch data from the downstream provider
                return await provider.GetFlightStatusAsync(flightNumber, date);
            }
            catch (Exception ex)
            {
                // Graceful Degradation: If a single provider fails or times out, it is caught here.
                // The error is logged so system health can be monitored, but it does NOT crash the overall request.
                // It simply returns null, allowing the MergeService to rely on the surviving providers.
                _logger.LogError(ex, "Provider {ProviderName} failed for flight {FlightNumber}", provider.ProviderName, flightNumber);
                return null;
            }
        });

        // Wait for all provider tasks to complete simultaneously (concurrency)
        var results = await Task.WhenAll(tasks);
        
        // 2. Gather Phase: Pass the array of all provider results (including nulls from failures) to the MergeService
        return _mergeService.Merge(results);
    }
}
