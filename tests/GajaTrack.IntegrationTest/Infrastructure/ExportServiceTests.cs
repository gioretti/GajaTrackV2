using System.Text.Json;
using GajaTrack.Domain.Entities;
using GajaTrack.Infrastructure.Persistence;
using GajaTrack.Infrastructure.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace GajaTrack.IntegrationTest.Infrastructure;

public class ExportServiceTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly GajaDbContext _context;

    public ExportServiceTests()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<GajaDbContext>()
            .UseSqlite(_connection)
            .Options;

        _context = new GajaDbContext(options);
        _context.Database.EnsureCreated();
    }

    [Fact]
    public async Task Export_ShouldHaveCorrectRootAndStructure()
    {
        // Arrange
        var service = new ExportService(_context);

        // Act
        var resultBytes = await service.ExportDataAsync();
        var jsonString = System.Text.Encoding.UTF8.GetString(resultBytes);
        using var doc = JsonDocument.Parse(jsonString);

        // Assert
        Assert.True(doc.RootElement.TryGetProperty("gajaTracking", out var root), "Root 'gajaTracking' missing");
        Assert.True(root.TryGetProperty("nursingFeeds", out _), "nursingFeeds missing");
        Assert.True(root.TryGetProperty("bottleFeeds", out _), "bottleFeeds missing");
        Assert.True(root.TryGetProperty("sleepSessions", out _), "sleepSessions missing");
        Assert.True(root.TryGetProperty("diaperChanges", out _), "diaperChanges missing");
    }

    [Fact]
    public async Task Export_ShouldSerializeNursingFeedCorrectly()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var feed = NursingFeed.Create(Guid.NewGuid(), "pk1", UtcDateTime.FromDateTime(now), UtcDateTime.FromDateTime(now.AddMinutes(15)));
        _context.NursingFeeds.Add(feed);
        await _context.SaveChangesAsync();
        var service = new ExportService(_context);

        // Act
        var resultBytes = await service.ExportDataAsync();
        using var doc = JsonDocument.Parse(resultBytes);
        var item = doc.RootElement.GetProperty("gajaTracking").GetProperty("nursingFeeds")[0];

        // Assert
        Assert.Equal(feed.Id.ToString(), item.GetProperty("id").GetString());
        Assert.EndsWith("Z", item.GetProperty("startTime").GetString());
        Assert.EndsWith("Z", item.GetProperty("endTime").GetString());
        
        // Verify time value match
        var startJson = item.GetProperty("startTime").GetDateTime();
        Assert.Equal(feed.StartTime, startJson);
    }

    [Fact]
    public async Task Export_ShouldSerializeBottleFeedCorrectly()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var feed = BottleFeed.Create(Guid.NewGuid(), "pk2", UtcDateTime.FromDateTime(now), 150, BottleContent.BreastMilk);
        _context.BottleFeeds.Add(feed);
        await _context.SaveChangesAsync();
        var service = new ExportService(_context);

        // Act
        var resultBytes = await service.ExportDataAsync();
        using var doc = JsonDocument.Parse(resultBytes);
        var item = doc.RootElement.GetProperty("gajaTracking").GetProperty("bottleFeeds")[0];

        // Assert
        Assert.Equal(feed.Id.ToString(), item.GetProperty("id").GetString());
        Assert.Equal(150, item.GetProperty("amountMl").GetInt32());
        Assert.Equal("BreastMilk", item.GetProperty("content").GetString()); // Enum as String
        Assert.EndsWith("Z", item.GetProperty("time").GetString());
    }

    [Fact]
    public async Task Export_ShouldSerializeSleepSessionCorrectly()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var sleep = SleepSession.Create(Guid.NewGuid(), "pk3", UtcDateTime.FromDateTime(now), UtcDateTime.FromDateTime(now.AddHours(2)));
        _context.SleepSessions.Add(sleep);
        await _context.SaveChangesAsync();
        var service = new ExportService(_context);

        // Act
        var resultBytes = await service.ExportDataAsync();
        using var doc = JsonDocument.Parse(resultBytes);
        var item = doc.RootElement.GetProperty("gajaTracking").GetProperty("sleepSessions")[0];

        // Assert
        Assert.Equal(sleep.Id.ToString(), item.GetProperty("id").GetString());
        Assert.EndsWith("Z", item.GetProperty("startTime").GetString());
        Assert.EndsWith("Z", item.GetProperty("endTime").GetString());
    }

    [Fact]
    public async Task Export_ShouldSerializeDiaperChangeCorrectly()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var diaper = DiaperChange.Create(Guid.NewGuid(), "pk4", UtcDateTime.FromDateTime(now), DiaperType.Soiled);
        _context.DiaperChanges.Add(diaper);
        await _context.SaveChangesAsync();
        var service = new ExportService(_context);

        // Act
        var resultBytes = await service.ExportDataAsync();
        using var doc = JsonDocument.Parse(resultBytes);
        var item = doc.RootElement.GetProperty("gajaTracking").GetProperty("diaperChanges")[0];

        // Assert
        Assert.Equal(diaper.Id.ToString(), item.GetProperty("id").GetString());
        Assert.Equal("Soiled", item.GetProperty("type").GetString()); // Enum as String
        Assert.EndsWith("Z", item.GetProperty("time").GetString());
    }

    public void Dispose()
    {
        _connection.Dispose();
        _context.Dispose();
    }
}
