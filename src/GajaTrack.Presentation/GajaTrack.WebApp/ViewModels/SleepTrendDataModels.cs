namespace GajaTrack.WebApp.ViewModels;

public enum SleepAggregationMode
{
    Daily,
    Weekly,
    Monthly
}

public record SleepTrendDataPoint(
    string Label,
    double NightSleepMinutes,
    double NapsMinutes
);
