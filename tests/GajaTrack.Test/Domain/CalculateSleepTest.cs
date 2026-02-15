using GajaTrack.Domain;
using GajaTrack.Domain.Entities;
using Xunit;

namespace GajaTrack.Test.Domain;

public class CalculateSleepTest
{
    private readonly CalculateSleep _calculator = new();
    
    // Window: 06:00 to 06:00 (Next Day)
    private readonly UtcDateTime _windowStart = UtcDateTime.FromDateTime(new DateTime(2026, 2, 15, 6, 0, 0, DateTimeKind.Utc));
    private readonly UtcDateTime _windowEnd = UtcDateTime.FromDateTime(new DateTime(2026, 2, 16, 6, 0, 0, DateTimeKind.Utc));
    private TimeRange TimeBounds => new(_windowStart, _windowEnd);

    [Fact]
    public void For_ShouldReturnClippedDuration_WhenSessionStartsBeforeWindow()
    {
        // Session: 05:00 - 07:00 (Only 06:00-07:00 is in window)
        var session = SleepSession.Create(
            Guid.NewGuid(),
            "ext-1",
            UtcDateTime.FromDateTime(_windowStart.Value.AddHours(-1)),
            UtcDateTime.FromDateTime(_windowStart.Value.AddHours(1))
        );
        var day = CreateBabyDay(new[] { session });

        var result = _calculator.For(day);

        Assert.Equal(60, result);
    }

    [Fact]
    public void For_ShouldReturnFullDuration_WhenSessionIsInsideWindow()
    {
        // Session: 10:00 - 11:30
        var session = SleepSession.Create(
            Guid.NewGuid(),
            "ext-2",
            UtcDateTime.FromDateTime(_windowStart.Value.AddHours(4)),
            UtcDateTime.FromDateTime(_windowStart.Value.AddHours(5.5))
        );
        var day = CreateBabyDay(new[] { session });

        var result = _calculator.For(day);

        Assert.Equal(90, result);
    }

    [Fact]
    public void For_ShouldReturnClippedDuration_WhenSessionEndsAfterWindow()
    {
        // Session: 05:00 AM (Next Day) - 07:00 AM (Next Day) (Only 05:00-06:00 is in window)
        var session = SleepSession.Create(
            Guid.NewGuid(),
            "ext-3",
            UtcDateTime.FromDateTime(_windowEnd.Value.AddHours(-1)),
            UtcDateTime.FromDateTime(_windowEnd.Value.AddHours(1))
        );
        var day = CreateBabyDay(new[] { session });

        var result = _calculator.For(day);

        Assert.Equal(60, result);
    }

    private BabyDay CreateBabyDay(IEnumerable<SleepSession> sessions)
    {
        return new BabyDay(
            TimeBounds,
            sessions,
            Enumerable.Empty<CryingSession>(),
            Enumerable.Empty<NursingFeed>(),
            Enumerable.Empty<BottleFeed>(),
            Enumerable.Empty<DiaperChange>()
        );
    }
}
