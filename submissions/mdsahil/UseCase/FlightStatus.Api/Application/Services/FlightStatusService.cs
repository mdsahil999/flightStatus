using FlightStatus.Api.Application.DTOs;
using FlightStatus.Api.Domain.Enums;
using FlightStatus.Api.Domain.Interfaces;

namespace FlightStatus.Api.Application.Services;

public class FlightStatusService(
    IFlightStatusRepository repository,
    StatusNormalizer normalizer)
{
    public async Task<FlightStatusResult> GetFlightStatusAsync(string flightNumber, DateTime date)
    {
        var merged = await repository.GetFlightDataAsync(flightNumber, date);

        if (merged == null)
        {
            return new FlightStatusResult(
                flightNumber,
                FlightStatusEnum.Unknown,
                date,
                null,
                date,
                null,
                null,
                null,
                "No usable status returned from providers.",
                DateTime.UtcNow
            );
        }

        var normalizedStatus = normalizer.Normalize(merged);

        return new FlightStatusResult(
            flightNumber,
            normalizedStatus,
            merged.ScheduledDeparture,
            merged.ActualDeparture,
            merged.ScheduledArrival,
            merged.ActualArrival,
            merged.Terminal,
            merged.Gate,
            merged.DelayReason,
            merged.LastUpdatedUtc
        );
    }
}
