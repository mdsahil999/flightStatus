using FlightStatus.Api.Application.DTOs;
using FlightStatus.Api.Domain.Enums;
using FlightStatus.Api.Domain.Interfaces;

namespace FlightStatus.Api.Application.Services;

public class FlightStatusService
{
    private readonly IFlightStatusRepository _repository;
    private readonly StatusNormalizer _normalizer;

    public FlightStatusService(
        IFlightStatusRepository repository,
        StatusNormalizer normalizer)
    {
        _repository = repository;
        _normalizer = normalizer;
    }

    public async Task<FlightStatusResult> GetFlightStatusAsync(string flightNumber, DateTime date)
    {
        var merged = await _repository.GetFlightDataAsync(flightNumber, date);

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

        var normalizedStatus = _normalizer.Normalize(merged);

        var delayReason = merged.DelayReason;
        
        // Enforce business rules for DelayReason
        if (normalizedStatus == FlightStatusEnum.Delayed)
        {
            if (string.IsNullOrWhiteSpace(delayReason))
            {
                delayReason = "Reason unavailable";
            }
        }
        else
        {
            // Only allow delay reason if status is explicitly Delayed
            delayReason = null;
        }

        return new FlightStatusResult(
            flightNumber,
            normalizedStatus,
            merged.ScheduledDeparture,
            merged.ActualDeparture,
            merged.ScheduledArrival,
            merged.ActualArrival,
            merged.Terminal,
            merged.Gate,
            delayReason,
            merged.LastUpdatedUtc
        );
    }
}
