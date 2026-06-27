using FlightStatus.Api.Domain.Entities;
using FlightStatus.Api.Domain.Enums;

namespace FlightStatus.Api.Application.Services;

public class StatusNormalizer
{
    public FlightStatusEnum Normalize(ProviderFlightStatus status)
    {
        var raw = status.Status;

        // 1. Check for Cancelled / Not Operating
        if (raw.Equals("Cancelled", StringComparison.OrdinalIgnoreCase) || 
            raw.Equals("NotOperating", StringComparison.OrdinalIgnoreCase))
        {
            return FlightStatusEnum.Cancelled;
        }

        // 2. Check for Diverted / Landed Elsewhere
        if (raw.Equals("Diverted", StringComparison.OrdinalIgnoreCase) || 
            raw.Equals("LandedElsewhere", StringComparison.OrdinalIgnoreCase))
        {
            return FlightStatusEnum.Diverted;
        }

        // 3. Time-based normalization (AeroTrack provides actual times)
        if (status.ActualDeparture.HasValue)
        {
            var diff = status.ActualDeparture.Value - status.ScheduledDeparture;
            if (diff.TotalMinutes >= 15)
            {
                return FlightStatusEnum.Delayed;
            }
        }
        
        if (status.ActualArrival.HasValue)
        {
            var diff = status.ActualArrival.Value - status.ScheduledArrival;
            if (diff.TotalMinutes >= 15)
            {
                return FlightStatusEnum.Delayed;
            }
        }

        // 4. Fallback for delay vocabularies (QuickFlight or missing times)
        if (raw.Equals("DepartedLate", StringComparison.OrdinalIgnoreCase) || 
            raw.Equals("RunningBehind", StringComparison.OrdinalIgnoreCase) ||
            raw.Equals("Delayed", StringComparison.OrdinalIgnoreCase))
        {
            return FlightStatusEnum.Delayed;
        }

        // 5. Default to OnTime for Scheduled, Boarding, Departed, OnSchedule
        return FlightStatusEnum.OnTime;
    }
}
