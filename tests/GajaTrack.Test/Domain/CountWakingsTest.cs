using GajaTrack.Domain;
using GajaTrack.Domain.Entities;
using Xunit;

namespace GajaTrack.Test.Domain;

public class CountWakingsTest
{
    private readonly CountWakings _counter = new();
    private readonly DateOnly _today = new(2026, 2, 15);
    
    // Window: 06:00 to 06:00 (Next Day)
    private readonly UtcDateTime _windowStart = UtcDateTime.FromDateTime(new DateTime(2026, 2, 15, 6, 0, 0, DateTimeKind.Utc));
    private readonly UtcDateTime _windowEnd = UtcDateTime.FromDateTime(new DateTime(2026, 2, 16, 6, 0, 0, DateTimeKind.Utc));
    private TimeRange Window => new(_windowStart, _windowEnd);

    [Fact]
    public void For_ShouldReturnZero_WhenNoWakingsInNightRange()
    {
        // Session ends at 14:00 (outside night range 18:00-06:00)
        var session = SleepSession.Create(Guid.NewGuid(), "ext-1", 
            UtcDateTime.FromDateTime(_windowStart.Value.AddHours(6)), 
            UtcDateTime.FromDateTime(_windowStart.Value.AddHours(8)));
        
        var day = CreateBabyDay(new[] { session });

        var result = _counter.For(day, new TimeOnly(18, 0), new TimeOnly(6, 0));

        Assert.Equal(0, result);
    }

    [Fact]
    public void For_ShouldCountInterruptions_WithinNightRange()
    {
        // Session 1: 19:00 - 22:00 (Ends in night, not last)
        var s1 = SleepSession.Create(Guid.NewGuid(), "ext-1", 
            UtcDateTime.FromDateTime(_windowStart.Value.AddHours(13)), 
            UtcDateTime.FromDateTime(_windowStart.Value.AddHours(16)));
        
        // Session 2: 00:00 - 02:00 (Ends in night, not last)
        var s2 = SleepSession.Create(Guid.NewGuid(), "ext-2", 
            UtcDateTime.FromDateTime(_windowStart.Value.AddHours(18)), 
            UtcDateTime.FromDateTime(_windowStart.Value.AddHours(20)));

        // Session 3: 04:00 - 07:00 (Last session, ends outside night)
        var s3 = SleepSession.Create(Guid.NewGuid(), "ext-3", 
            UtcDateTime.FromDateTime(_windowStart.Value.AddHours(22)), 
            UtcDateTime.FromDateTime(_windowStart.Value.AddHours(25)));

        var day = CreateBabyDay(new[] { s1, s2, s3 });

        var result = _counter.For(day, new TimeOnly(18, 0), new TimeOnly(6, 0));

        // Interruption 1: at 22:00
        // Interruption 2: at 02:00
        Assert.Equal(2, result);
    }

    [Fact]
    public void For_ShouldExcludeLastSession_EvenIfItEndsInNightRange()
    {
        // Session 1: 19:00 - 22:00 (Ends in night)
        var s1 = SleepSession.Create(Guid.NewGuid(), "ext-1", 
            UtcDateTime.FromDateTime(_windowStart.Value.AddHours(13)), 
            UtcDateTime.FromDateTime(_windowStart.Value.AddHours(16)));
        
        // Session 2: 23:00 - 01:00 (Last session, ends in night)
        var s2 = SleepSession.Create(Guid.NewGuid(), "ext-2", 
            UtcDateTime.FromDateTime(_windowStart.Value.AddHours(17)), 
            UtcDateTime.FromDateTime(_windowStart.Value.AddHours(19)));

        var day = CreateBabyDay(new[] { s1, s2 });

        var result = _counter.For(day, new TimeOnly(18, 0), new TimeOnly(6, 0));

        // Only s1 counts. s2 is excluded as the last session.
        Assert.Equal(1, result);
    }

    private BabyDay CreateBabyDay(IEnumerable<SleepSession> sessions)
    {
        return new BabyDay(
            _today,
            Window,
            sessions,
            Enumerable.Empty<CryingSession>(),
            Enumerable.Empty<NursingFeed>(),
            Enumerable.Empty<BottleFeed>(),
            Enumerable.Empty<DiaperChange>()
        );
    }
}
