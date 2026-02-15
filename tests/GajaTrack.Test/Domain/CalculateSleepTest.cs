using GajaTrack.Domain;
using GajaTrack.Domain.Entities;
using Xunit;

namespace GajaTrack.Test.Domain;

public class CalculateSleepTest
{
    private readonly CalculateSleep _calculator = new();
    
    // Test logical day: Feb 15, 06:00 to Feb 16, 06:00 (UTC for simplicity)
    private readonly UtcDateTime _windowStart = UtcDateTime.FromDateTime(new DateTime(2026, 2, 15, 6, 0, 0, DateTimeKind.Utc));
    private readonly UtcDateTime _windowEnd = UtcDateTime.FromDateTime(new DateTime(2026, 2, 16, 6, 0, 0, DateTimeKind.Utc));
    private TimeRange TimeBounds => new(_windowStart, _windowEnd);

    [Fact]
    public void For_ShouldCategorizeStandardNap_Correctly()
    {
        // 10:00 - 11:00 (60 mins)
        var session = SleepSession.Create(Guid.NewGuid(), "ext", 
            UtcDateTime.FromDateTime(_windowStart.Value.AddHours(4)), 
            UtcDateTime.FromDateTime(_windowStart.Value.AddHours(5)));
        
        var result = _calculator.For(CreateBabyDay(new[] { session }), TimeZoneInfo.Utc);

        Assert.Equal(60, result.NapsMinutes);
        Assert.Equal(0, result.NightSleepMinutes);
    }

    [Fact]
    public void For_ShouldCategorizeStandardNight_Correctly()
    {
        // 22:00 - 02:00 (Next day) (240 mins)
        var session = SleepSession.Create(Guid.NewGuid(), "ext", 
            UtcDateTime.FromDateTime(_windowStart.Value.AddHours(16)), 
            UtcDateTime.FromDateTime(_windowStart.Value.AddHours(20)));
        
        var result = _calculator.For(CreateBabyDay(new[] { session }), TimeZoneInfo.Utc);

        Assert.Equal(0, result.NapsMinutes);
        Assert.Equal(240, result.NightSleepMinutes);
    }

    [Fact]
    public void For_ShouldCategorizeBoundaryNap_FullyAsNap()
    {
        // Start 17:59, End 18:30 (31 mins)
        var session = SleepSession.Create(Guid.NewGuid(), "ext", 
            UtcDateTime.FromDateTime(_windowStart.Value.AddHours(11).AddMinutes(59)), 
            UtcDateTime.FromDateTime(_windowStart.Value.AddHours(12).AddMinutes(30)));
        
        var result = _calculator.For(CreateBabyDay(new[] { session }), TimeZoneInfo.Utc);

        Assert.Equal(31, result.NapsMinutes);
        Assert.Equal(0, result.NightSleepMinutes);
    }

    [Fact]
    public void For_ShouldCategorizeBoundaryNight_FullyAsNight()
    {
        // Start 05:59 (Next day morning), End 07:00 (61 mins)
        var session = SleepSession.Create(Guid.NewGuid(), "ext", 
            UtcDateTime.FromDateTime(_windowEnd.Value.AddMinutes(-1)), 
            UtcDateTime.FromDateTime(_windowEnd.Value.AddHours(1)));
        
        var result = _calculator.For(CreateBabyDay(new[] { session }), TimeZoneInfo.Utc);

        Assert.Equal(0, result.NapsMinutes);
        Assert.Equal(61, result.NightSleepMinutes);
    }

    [Fact]
    public void For_ShouldExcludeSession_StartingBeforeDayBounds()
    {
        // Start 05:50 (belongs to previous logical day)
        var session = SleepSession.Create(Guid.NewGuid(), "ext", 
            UtcDateTime.FromDateTime(_windowStart.Value.AddMinutes(-10)), 
            UtcDateTime.FromDateTime(_windowStart.Value.AddHours(1)));
        
        var result = _calculator.For(CreateBabyDay(new[] { session }), TimeZoneInfo.Utc);

        Assert.Equal(0, result.NapsMinutes);
        Assert.Equal(0, result.NightSleepMinutes);
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
