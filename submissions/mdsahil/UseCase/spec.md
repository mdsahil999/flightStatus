# Flight Status Data Model & Interfaces

## Enums
```csharp
public enum FlightStatusEnum
{
    OnTime,
    Delayed,
    Cancelled,
    Diverted,
    Unknown
}
```

## DTOs
```csharp
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
}
```

## Provider Interfaces
```csharp
public interface IFlightStatusProvider
{
    string ProviderName { get; }
    Task<ProviderFlightStatus?> GetFlightStatusAsync(string flightNumber, DateTime date);
}

// Unified internal model returned by providers before normalization
public class ProviderFlightStatus
{
    public string FlightNumber { get; set; }
    public string Status { get; set; }
    public DateTime ScheduledDeparture { get; set; }
    public DateTime? ActualDeparture { get; set; }
    public DateTime ScheduledArrival { get; set; }
    public DateTime? ActualArrival { get; set; }
    public string? Terminal { get; set; }
    public string? Gate { get; set; }
    public string? DelayReason { get; set; }
    public DateTime LastUpdatedUtc { get; set; }
}
```

## Repositories
```csharp
public interface IFlightStatusRepository
{
    // Aggregates data from all IFlightStatusProviders
    Task<ProviderFlightStatus?> GetFlightDataAsync(string flightNumber, DateTime date);
}
```
