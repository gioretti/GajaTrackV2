using GajaTrack.Application.DTOs.Protocol;
using GajaTrack.Domain.Entities;
using GajaTrack.Infrastructure.Persistence;
using GajaTrack.Infrastructure.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace GajaTrack.Tests.Integration.Services;

public class ProtocolServiceTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly GajaDbContext _context;

    public ProtocolServiceTests()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<GajaDbContext>()
            .UseSqlite(_connection)
            .Options;

        _context = new GajaDbContext(options);
        _context.Database.EnsureCreated();
    }

    public void Dispose()
    {
        _connection.Dispose();
        _context.Dispose();
    }

    [Fact]
    public async Task GetProtocol_ShouldIncludeEventsInsideWindow()
    {
        // Arrange
        // Day: Feb 5th (06:00 Feb 5 - 06:00 Feb 6)
        var day = new DateOnly(2026, 2, 5); 
        
        // 10:00 - 12:00 (Inside)
        var sleepStart = new DateTime(2026, 2, 5, 10, 0, 0, DateTimeKind.Utc);
        var sleepEnd = new DateTime(2026, 2, 5, 12, 0, 0, DateTimeKind.Utc);
        
        _context.SleepSessions.Add(SleepSession.Create(Guid.NewGuid(), "1", sleepStart, sleepEnd));
        await _context.SaveChangesAsync();
        
        var service = new ProtocolService(_context);

        // Act
        var result = await service.GetProtocolAsync(day, day);

        // Assert
        Assert.Single(result);
        var protocolDay = result[0];
        Assert.Equal(day, protocolDay.Date);
        
        Assert.Single(protocolDay.Events);
        var ev = protocolDay.Events[0];
        Assert.Equal(ProtocolEventType.Sleep, ev.Type);
        
        // 06:00 -> 10:00 = 4 hours = 240 minutes
        Assert.Equal(240, ev.StartMinute); 
        Assert.Equal(120, ev.DurationMinutes);
    }

    [Fact]
    public async Task GetProtocol_ShouldSplitEventCrossingStartBoundary()
    {
        // Arrange
        // Day: Feb 5th (06:00 Feb 5 - 06:00 Feb 6)
        var day = new DateOnly(2026, 2, 5); 
        
        // Starts Feb 5 05:00, Ends Feb 5 07:00
        // Should appear as 06:00 - 07:00 (StartMinute 0, Duration 60)
        var sleepStart = new DateTime(2026, 2, 5, 5, 0, 0, DateTimeKind.Utc);
        var sleepEnd = new DateTime(2026, 2, 5, 7, 0, 0, DateTimeKind.Utc);
        
        _context.SleepSessions.Add(SleepSession.Create(Guid.NewGuid(), "split-start", sleepStart, sleepEnd));
        await _context.SaveChangesAsync();
        
        var service = new ProtocolService(_context);

        // Act
        var result = await service.GetProtocolAsync(day, day);

        // Assert
        Assert.Single(result);
        Assert.Single(result[0].Events);
        var ev = result[0].Events[0];
        
        Assert.Equal(0, ev.StartMinute); // Starts at 06:00 (window start)
        Assert.Equal(60, ev.DurationMinutes); // 1 hour duration in this window
    }

    [Fact]
    public async Task GetProtocol_ShouldSplitEventCrossingEndBoundary()
    {
        // Arrange
        // Day: Feb 5th (06:00 Feb 5 - 06:00 Feb 6)
        var day = new DateOnly(2026, 2, 5); 
        
        // Starts Feb 6 05:00, Ends Feb 6 07:00
        // Should appear as 05:00 - 06:00 (StartMinute 23*60 = 1380, Duration 60)
        var sleepStart = new DateTime(2026, 2, 6, 5, 0, 0, DateTimeKind.Utc);
        var sleepEnd = new DateTime(2026, 2, 6, 7, 0, 0, DateTimeKind.Utc);
        
        _context.SleepSessions.Add(SleepSession.Create(Guid.NewGuid(), "split-end", sleepStart, sleepEnd));
        await _context.SaveChangesAsync();
        
        var service = new ProtocolService(_context);

        // Act
        var result = await service.GetProtocolAsync(day, day);

        // Assert
        Assert.Single(result);
        Assert.Single(result[0].Events);
        var ev = result[0].Events[0];
        
        // 05:00 is 23 hours after 06:00 (prev day)
        Assert.Equal(23 * 60, ev.StartMinute); 
        Assert.Equal(60, ev.DurationMinutes);
    }
    
    [Fact]
    public async Task GetProtocol_ShouldIncludePointEvents()
    {
        // Arrange
        var day = new DateOnly(2026, 2, 5);
        
        // Nursing at 08:00
        var nursingTime = new DateTime(2026, 2, 5, 8, 0, 0, DateTimeKind.Utc);
        
        _context.NursingFeeds.Add(NursingFeed.Create(Guid.NewGuid(), "n1", nursingTime, null));
        await _context.SaveChangesAsync();
        
        var service = new ProtocolService(_context);

        // Act
        var result = await service.GetProtocolAsync(day, day);

        // Assert
        Assert.Single(result[0].Events);
        var ev = result[0].Events[0];
        Assert.Equal(ProtocolEventType.Nursing, ev.Type);
        Assert.Equal(120, ev.StartMinute); // 2 hours
        Assert.Equal(0, ev.DurationMinutes);
    }
}
