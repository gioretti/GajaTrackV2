using FluentAssertions;
using GajaTrack.Application.DTOs.DailyRhythmMap;
using GajaTrack.WebApp.ViewModels;

namespace GajaTrack.Test.ViewModels;

public class SleepTrendAggregatorTests
{
    private DailyRhythmMapDay CreateDay(DateTime date, double nightSleepMinutes, double napsMinutes)
    {
        var dateOnly = new DateOnly(date.Year, date.Month, date.Day);
        var windowStart = date.Date.AddHours(6);
        return new DailyRhythmMapDay(
            Date: dateOnly,
            WindowStart: windowStart,
            WindowEnd: windowStart.AddDays(1),
            Events: new List<DailyRhythmMapEvent>(),
            Summary: new DailyRhythmMapSummary(
                NapsMinutes: napsMinutes,
                NightSleepMinutes: nightSleepMinutes,
                NightWakingCount: 0
            )
        );
    }

    [Fact]
    public void Aggregate_WithEmptyList_ReturnsEmpty()
    {
        var result = SleepTrendAggregator.Aggregate(new List<DailyRhythmMapDay>(), SleepAggregationMode.Daily);
        result.Should().BeEmpty();
    }

    [Fact]
    public void AggregateDaily_MapsDataCorrectly()
    {
        var days = new List<DailyRhythmMapDay>
        {
            CreateDay(new DateTime(2023, 10, 1), 600, 120),
            CreateDay(new DateTime(2023, 10, 2), 660, 60)
        };

        var result = SleepTrendAggregator.Aggregate(days, SleepAggregationMode.Daily).ToList();

        result.Should().HaveCount(2);

        result[0].Label.Should().Be("01.10");
        result[0].NightSleepMinutes.Should().Be(600);
        result[0].NapsMinutes.Should().Be(120);

        result[1].Label.Should().Be("02.10");
        result[1].NightSleepMinutes.Should().Be(660);
        result[1].NapsMinutes.Should().Be(60);
    }

    [Fact]
    public void AggregateWeekly_AveragesDataPerIsoWeek()
    {
        var days = new List<DailyRhythmMapDay>
        {
            // Week 40
            CreateDay(new DateTime(2023, 10, 2), 600, 120), // Monday
            CreateDay(new DateTime(2023, 10, 3), 660, 60), // Tuesday

            // Week 41
            CreateDay(new DateTime(2023, 10, 9), 500, 0), // Monday
        };

        var result = SleepTrendAggregator.Aggregate(days, SleepAggregationMode.Weekly).ToList();

        result.Should().HaveCount(2);

        // Week 40 Average (600+660)/2 = 630 night, (120+60)/2 = 90 naps
        result[0].Label.Should().Be("Wk 40");
        result[0].NightSleepMinutes.Should().Be(630);
        result[0].NapsMinutes.Should().Be(90);

        // Week 41
        result[1].Label.Should().Be("Wk 41");
        result[1].NightSleepMinutes.Should().Be(500);
        result[1].NapsMinutes.Should().Be(0);
    }

    [Fact]
    public void AggregateMonthly_AveragesDataPerMonth()
    {
        var days = new List<DailyRhythmMapDay>
        {
            // October
            CreateDay(new DateTime(2023, 10, 15), 600, 120),
            CreateDay(new DateTime(2023, 10, 25), 660, 60),

            // November
            CreateDay(new DateTime(2023, 11, 5), 500, 0),
        };

        var result = SleepTrendAggregator.Aggregate(days, SleepAggregationMode.Monthly).ToList();

        result.Should().HaveCount(2);

        // October Average (600+660)/2 = 630 night, (120+60)/2 = 90 naps
        result[0].Label.Should().Be("Oct");
        result[0].NightSleepMinutes.Should().Be(630);
        result[0].NapsMinutes.Should().Be(90);

        // November
        result[1].Label.Should().Be("Nov");
        result[1].NightSleepMinutes.Should().Be(500);
        result[1].NapsMinutes.Should().Be(0);
    }

    [Fact]
    public void AggregateWeekly_HandlesYearBoundaryCorrectly()
    {
        var days = new List<DailyRhythmMapDay>
        {
            // Jan 1 2023 was a Sunday. ISO week is still part of 2022 week 52.
            CreateDay(new DateTime(2023, 1, 1), 600, 120),

            // Jan 2 2023 is Monday -> Week 1 of 2023
            CreateDay(new DateTime(2023, 1, 2), 660, 60),
        };

        var result = SleepTrendAggregator.Aggregate(days, SleepAggregationMode.Weekly).ToList();

        result.Should().HaveCount(2);

        result[0].Label.Should().Be("Wk 52");
        result[1].Label.Should().Be("Wk 1");
    }
}
