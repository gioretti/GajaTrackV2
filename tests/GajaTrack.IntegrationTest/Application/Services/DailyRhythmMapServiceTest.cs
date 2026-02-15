using GajaTrack.Application.DTOs.DailyRhythmMap;
using GajaTrack.Application.Queries;
using GajaTrack.Application.Services;
using GajaTrack.Domain;
using GajaTrack.Domain.Entities;
using GajaTrack.Infrastructure.Persistence;
using GajaTrack.Infrastructure.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace GajaTrack.Application.Services;

public class DailyRhythmMapServiceTest : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly GajaDbContext _context;
    private readonly DailyRhythmMapService _service;

    public DailyRhythmMapServiceTest()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<GajaDbContext>()
            .UseSqlite(_connection)
            .Options;

        _context = new GajaDbContext(options);
        _context.Database.EnsureCreated();

        var repository = new TrackingRepository(_context);
        var getBabyDayExecution = new GetBabyDay.Execution(repository);
        var calculateSleep = new CalculateSleep();
        var countWakings = new CountWakings();
        _service = new DailyRhythmMapService(getBabyDayExecution, calculateSleep, countWakings);
    }

    public void Dispose()
    {
        _connection.Dispose();
        _context.Dispose();
    }

    [Fact]
    public async Task GetDailyRhythmMap_ShouldIncludeEventsInsideWindow()
    {
        // Arrange
        // Day: Feb 5th (06:00 Feb 5 - 06:00 Feb 6)
        var day = new DateOnly(2026, 2, 5); 
        
        // 10:00 - 12:00 (Inside)
        var sleepStart = UtcDateTime.FromDateTime(new DateTime(2026, 2, 5, 10, 0, 0, DateTimeKind.Utc));
        var sleepEnd = UtcDateTime.FromDateTime(new DateTime(2026, 2, 5, 12, 0, 0, DateTimeKind.Utc));
        
        _context.SleepSessions.Add(SleepSession.Create(Guid.NewGuid(), "1", sleepStart, sleepEnd));
        await _context.SaveChangesAsync();
        
        // Act
        var result = await _service.GetDailyRhythmMapAsync(day, day, timeZone: TimeZoneInfo.Utc);

        // Assert
        Assert.Single(result);
        var dailyRhythmMapDay = result[0];
        Assert.Equal(day, dailyRhythmMapDay.Date);
        
        Assert.Single(dailyRhythmMapDay.Events);
        var ev = dailyRhythmMapDay.Events[0];
        Assert.Equal(DailyRhythmMapEventType.Sleep, ev.Type);
        
        // 06:00 -> 10:00 = 4 hours = 240 minutes
        Assert.Equal(240, ev.StartMinute); 
        Assert.Equal(120, ev.DurationMinutes);
    }

    [Fact]
    public async Task GetDailyRhythmMap_ShouldCategorizeSleep_IntoNapsAndNight()
    {
        // Arrange
        var day = new DateOnly(2026, 2, 5); // 06:00 - 06:00
        
        // Nap: 10:00 - 11:00 (60 mins)
        _context.SleepSessions.Add(SleepSession.Create(Guid.NewGuid(), "nap", 
            UtcDateTime.FromDateTime(new DateTime(2026, 2, 5, 10, 0, 0, DateTimeKind.Utc)),
            UtcDateTime.FromDateTime(new DateTime(2026, 2, 5, 11, 0, 0, DateTimeKind.Utc))));
            
        // Night: 20:00 - 22:00 (120 mins)
        _context.SleepSessions.Add(SleepSession.Create(Guid.NewGuid(), "night", 
            UtcDateTime.FromDateTime(new DateTime(2026, 2, 5, 20, 0, 0, DateTimeKind.Utc)),
            UtcDateTime.FromDateTime(new DateTime(2026, 2, 5, 22, 0, 0, DateTimeKind.Utc))));
            
        await _context.SaveChangesAsync();
        
        // Act
        var result = await _service.GetDailyRhythmMapAsync(day, day, timeZone: TimeZoneInfo.Utc);

        // Assert
        Assert.Equal(60, result[0].Summary.NapsMinutes);
        Assert.Equal(120, result[0].Summary.NightSleepMinutes);
        Assert.Equal(180, result[0].Summary.TotalSleepMinutes);
    }

    [Fact]
    public async Task GetDailyRhythmMap_ShouldSplitEventCrossingStartBoundary()
    {
        // Arrange
        // Day: Feb 5th (06:00 Feb 5 - 06:00 Feb 6)
        var day = new DateOnly(2026, 2, 5); 
        
        // Starts Feb 5 05:00, Ends Feb 5 07:00
        // Should appear as 06:00 - 07:00 (StartMinute 0, Duration 60)
        var sleepStart = UtcDateTime.FromDateTime(new DateTime(2026, 2, 5, 5, 0, 0, DateTimeKind.Utc));
        var sleepEnd = UtcDateTime.FromDateTime(new DateTime(2026, 2, 5, 7, 0, 0, DateTimeKind.Utc));
        
        _context.SleepSessions.Add(SleepSession.Create(Guid.NewGuid(), "split-start", sleepStart, sleepEnd));
        await _context.SaveChangesAsync();
        
        // Act
        var result = await _service.GetDailyRhythmMapAsync(day, day, timeZone: TimeZoneInfo.Utc);

        // Assert
        Assert.Single(result);
        Assert.Single(result[0].Events);
        var ev = result[0].Events[0];
        
        Assert.Equal(0, ev.StartMinute); // Starts at 06:00 (window start)
        Assert.Equal(60, ev.DurationMinutes); // 1 hour duration in this window
    }

    [Fact]
    public async Task GetDailyRhythmMap_ShouldReturnMostRecentFirst_WhenDescendingIsTrue()
    {
        // Arrange
        var day1 = new DateOnly(2026, 2, 5);
        var day2 = new DateOnly(2026, 2, 6);
        
        // Act
        var result = await _service.GetDailyRhythmMapAsync(day1, day2, mostRecentFirst: true);

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Equal(day2, result[0].Date);
        Assert.Equal(day1, result[1].Date);
    }

    [Fact]
    public async Task GetDailyRhythmMap_ShouldCalculateTotalSleepMinutes()
    {
        // Arrange
        var day = new DateOnly(2026, 2, 5);
        
        // Sleep 1: 08:00 - 10:00 (120 mins)
        var s1Start = UtcDateTime.FromDateTime(new DateTime(2026, 2, 5, 8, 0, 0, DateTimeKind.Utc));
        var s1End = UtcDateTime.FromDateTime(new DateTime(2026, 2, 5, 10, 0, 0, DateTimeKind.Utc));
        
        // Sleep 2: 14:00 - 15:30 (90 mins)
        var s2Start = UtcDateTime.FromDateTime(new DateTime(2026, 2, 5, 14, 0, 0, DateTimeKind.Utc));
        var s2End = UtcDateTime.FromDateTime(new DateTime(2026, 2, 5, 15, 30, 0, DateTimeKind.Utc));
        
        _context.SleepSessions.Add(SleepSession.Create(Guid.NewGuid(), "s1", s1Start, s1End));
        _context.SleepSessions.Add(SleepSession.Create(Guid.NewGuid(), "s2", s2Start, s2End));
        await _context.SaveChangesAsync();
        
        // Act
        var result = await _service.GetDailyRhythmMapAsync(day, day, timeZone: TimeZoneInfo.Utc);

        // Assert
        Assert.Equal(120 + 90, result[0].Summary.TotalSleepMinutes);
    }

    [Fact]
    public async Task GetDailyRhythmMap_ShouldCalculateNightWakings_ExcludingLastWakeUp()
    {
        // Arrange
        var day = new DateOnly(2026, 2, 5); // 06:00 - 06:00 (Next Day)
        
        // Session 1: 20:00 - 02:00 (Ends at 02:00 next day)
        var s1Start = UtcDateTime.FromDateTime(new DateTime(2026, 2, 5, 20, 0, 0, DateTimeKind.Utc));
        var s1End = UtcDateTime.FromDateTime(new DateTime(2026, 2, 6, 2, 0, 0, DateTimeKind.Utc));
        
        // Session 2: 03:00 - 05:45 (Ends at 05:45 next day)
        var s2Start = UtcDateTime.FromDateTime(new DateTime(2026, 2, 6, 3, 0, 0, DateTimeKind.Utc));
        var s2End = UtcDateTime.FromDateTime(new DateTime(2026, 2, 6, 5, 45, 0, DateTimeKind.Utc));
        
        _context.SleepSessions.Add(SleepSession.Create(Guid.NewGuid(), "s5", s1Start, s1End));
        _context.SleepSessions.Add(SleepSession.Create(Guid.NewGuid(), "s6", s2Start, s2End));
        await _context.SaveChangesAsync();
        
        // Act
        var result = await _service.GetDailyRhythmMapAsync(day, day, timeZone: TimeZoneInfo.Utc);

        // Assert
        Assert.Equal(1, result[0].Summary.NightWakingCount);
    }
}
