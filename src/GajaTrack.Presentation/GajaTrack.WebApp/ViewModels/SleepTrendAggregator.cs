using System.Globalization;
using GajaTrack.Application.DTOs.DailyRhythmMap;

namespace GajaTrack.WebApp.ViewModels;

public static class SleepTrendAggregator
{
    public static IEnumerable<SleepTrendDataPoint> Aggregate(IEnumerable<DailyRhythmMapDay> days, SleepAggregationMode mode)
    {
        if (days == null || !days.Any())
        {
            return Enumerable.Empty<SleepTrendDataPoint>();
        }

        return mode switch
        {
            SleepAggregationMode.Daily => AggregateDaily(days),
            SleepAggregationMode.Weekly => AggregateWeekly(days),
            SleepAggregationMode.Monthly => AggregateMonthly(days),
            _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null)
        };
    }

    private static IEnumerable<SleepTrendDataPoint> AggregateDaily(IEnumerable<DailyRhythmMapDay> days)
    {
        return days.Select(d => new SleepTrendDataPoint(
            Label: d.Date.ToString("dd.MM"),
            NightSleepMinutes: d.Summary.NightSleepMinutes,
            NapsMinutes: d.Summary.NapsMinutes
        ));
    }

    private static IEnumerable<SleepTrendDataPoint> AggregateWeekly(IEnumerable<DailyRhythmMapDay> days)
    {
        return days.GroupBy(d => GetIso8601YearWeek(d.Date.ToDateTime(TimeOnly.MinValue)))
            .Select(g => new SleepTrendDataPoint(
                Label: $"Wk {g.Key.Week}",
                NightSleepMinutes: g.Average(d => d.Summary.NightSleepMinutes),
                NapsMinutes: g.Average(d => d.Summary.NapsMinutes)
            ));
    }

    private static IEnumerable<SleepTrendDataPoint> AggregateMonthly(IEnumerable<DailyRhythmMapDay> days)
    {
        return days.GroupBy(d => new { d.Date.Year, d.Date.Month })
            .Select(g => new SleepTrendDataPoint(
                Label: new DateTime(g.Key.Year, g.Key.Month, 1).ToString("MMM"), // e.g. Oct
                NightSleepMinutes: g.Average(d => d.Summary.NightSleepMinutes),
                NapsMinutes: g.Average(d => d.Summary.NapsMinutes)
            ));
    }

    private static (int Year, int Week) GetIso8601YearWeek(DateTime date)
    {
        var calendar = CultureInfo.InvariantCulture.Calendar;
        
        // Seriously cheat.  If its Monday, Tuesday or Wednesday, then it'll 
        // be the same week# as whatever Thursday, Friday or Saturday are,
        // and we always get those right
        DayOfWeek day = calendar.GetDayOfWeek(date);
        if (day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday)
        {
            date = date.AddDays(3);
        }

        // Return the week of our adjusted day
        int week = calendar.GetWeekOfYear(date, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        
        // Determine the year for the week number to handle week 52/53 vs week 1 boundary
        int year = date.Year;
        if (week == 1 && date.Month == 12)
        {
             year++;
        }
        else if (week >= 52 && date.Month == 1)
        {
             year--;
        }

        return (year, week);
    }
}
