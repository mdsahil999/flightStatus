using FlightStatus.Api.Domain.Entities;
using FlightStatus.Api.Domain.Enums;

namespace FlightStatus.Api.Application.Services;

public class StatusNormalizer
{
    public FlightStatusEnum Normalize(ProviderFlightStatus status)
    {
        if (status.Status.Equals("Cancelled", StringComparison.OrdinalIgnoreCase))
            return FlightStatusEnum.Cancelled;

        if (status.Status.Equals("Diverted", StringComparison.OrdinalIgnoreCase))
            return FlightStatusEnum.Diverted;

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
        else if (status.Status.Equals("Delayed", StringComparison.OrdinalIgnoreCase))
        {
            return FlightStatusEnum.Delayed;
        }

        return FlightStatusEnum.OnTime;
    }
}
