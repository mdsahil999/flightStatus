using FlightStatus.Api.Domain.Entities;

namespace FlightStatus.Api.Application.Services;

public class MergeService
{
    public ProviderFlightStatus? Merge(IEnumerable<ProviderFlightStatus?> results)
    {
        return results
            .Where(r => r != null)
            .OrderByDescending(r => r!.LastUpdatedUtc)
            .FirstOrDefault();
    }
}
