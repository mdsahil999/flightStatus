using FlightStatus.Api.Domain.Enums;

namespace FlightStatus.Api.Application.DTOs;

public class FlightStatusResult
{
    public string FlightNumber { get; set; }
    public FlightStatusEnum Status { get; set; }
    public DateTime ScheduledDeparture { get; set; }
    public DateTime? ActualDeparture { get; set; }
    public DateTime ScheduledArrival { get; set; }
    public DateTime? ActualArrival { get; set; }
    public string? Terminal { get; set; }
    public string? Gate { get; set; }
    public string? DelayReason { get; set; }
    public DateTime LastUpdatedUtc { get; set; }

    public FlightStatusResult(
        string FlightNumber,
        FlightStatusEnum Status,
        DateTime ScheduledDeparture,
        DateTime? ActualDeparture,
        DateTime ScheduledArrival,
        DateTime? ActualArrival,
        string? Terminal,
        string? Gate,
        string? DelayReason,
        DateTime LastUpdatedUtc)
    {
        this.FlightNumber = FlightNumber;
        this.Status = Status;
        this.ScheduledDeparture = ScheduledDeparture;
        this.ActualDeparture = ActualDeparture;
        this.ScheduledArrival = ScheduledArrival;
        this.ActualArrival = ActualArrival;
        this.Terminal = Terminal;
        this.Gate = Gate;
        this.DelayReason = DelayReason;
        this.LastUpdatedUtc = LastUpdatedUtc;
    }
}
