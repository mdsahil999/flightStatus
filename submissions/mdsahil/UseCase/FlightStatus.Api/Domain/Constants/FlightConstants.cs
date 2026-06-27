namespace FlightStatus.Api.Domain.Constants
{
    public class FlightConstants
    {
        public static readonly TimeSpan ScheduledDepartureTime = TimeSpan.FromHours(8);
        public static readonly TimeSpan FlightDuration = TimeSpan.FromHours(4);
        public static readonly TimeSpan DelayDuration = TimeSpan.FromMinutes(30);
    }
}
