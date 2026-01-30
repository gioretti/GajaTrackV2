using GajaTrack.Infrastructure.Persistence;
using GajaTrack.Infrastructure.Services;
using GajaTrack.Domain.Enums;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace GajaTrack.Tests.Integration;

public class LegacyImportTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly GajaDbContext _context;

    public LegacyImportTests()
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
    public async Task Import_ShouldParseJsonAndPersistEntities()
    {
        // Arrange
        var json = """
        {
           "baby_nursingfeed": [
             { "pk": "123", "startDate": 1700000000.0, "endDate": 1700000600.0 }
           ],
           "baby_bottlefeed": [
             { "pk": "456", "date": 1700001000.0, "amountML": 120, "isFormula": 1 }
           ],
           "baby_sleep": [
             { "pk": "789", "startDate": 1700002000.0, "endDate": 1700005000.0 }
           ],
           "baby_nappy": [
             { "pk": "101", "date": 1700006000.0, "type": "Wet" }
           ]
        }
        """;
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));
        var service = new LegacyImportService(_context);

        // Act
        var result = await service.ImportFromStreamAsync(stream);

        // Assert
        Assert.Equal(1, result.NursingFeedsImported);
        Assert.Equal(1, result.BottleFeedsImported);
        Assert.Equal(1, result.SleepSessionsImported);
        Assert.Equal(1, result.DiaperChangesImported);

        Assert.Single(_context.NursingFeeds);
        Assert.Single(_context.BottleFeeds);
        Assert.Single(_context.SleepSessions);
        Assert.Single(_context.DiaperChanges);
    }

    [Fact]
    public async Task Import_ShouldParseAndroidFormat_WithIsoDates()
    {
        // Arrange
        var json = """
        {
           "baby_nursingfeed": [
             { "pk": "A1", "startDate": "2023-11-15T10:00:00Z", "endDate": "2023-11-15T10:10:00Z" }
           ],
           "baby_bottlefeed": [
             { "pk": "A2", "date": "2023-11-15T12:00:00Z", "amountML": 150, "isFormula": true }
           ],
           "baby_sleep": [
             { "pk": "A3", "startDate": "2023-11-15T13:00:00Z", "endDate": "2023-11-15T15:00:00Z" }
           ],
           "baby_nappy": [
             { "pk": "A4", "date": "2023-11-15T16:00:00Z", "type": "Soiled" }
           ]
        }
        """;
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));
        var service = new LegacyImportService(_context);

        // Act
        var result = await service.ImportFromStreamAsync(stream);

        // Assert
        Assert.Equal(1, result.NursingFeedsImported);
        Assert.Equal(1, result.BottleFeedsImported);
        Assert.Equal(1, result.SleepSessionsImported);
        Assert.Equal(1, result.DiaperChangesImported);

        var feed = await _context.NursingFeeds.FirstAsync(x => x.ExternalId == "A1");
        Assert.Equal(new DateTime(2023, 11, 15, 10, 0, 0, DateTimeKind.Utc), feed.StartTime);
    }

    [Fact]
    public async Task Import_ShouldMapAllFieldsCorrectly()
    {
        // Arrange
        var json = """
        {
           "baby_nursingfeed": [{ "pk": "N1", "startDate": 1700000000.0, "endDate": 1700000600.0 }],
           "baby_bottlefeed": [{ "pk": "B1", "date": 1700001000.0, "amountML": 125, "isFormula": 1 }],
           "baby_sleep": [{ "pk": "S1", "startDate": 1700002000.0, "endDate": 1700005000.0 }],
           "baby_nappy": [
                { "pk": "D1", "date": 1700006000.0, "type": "Wet" },
                { "pk": "D2", "date": 1700007000.0, "type": "Soiled" },
                { "pk": "D3", "date": 1700008000.0, "type": "Mixed" }
           ]
        }
        """;
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));
        var service = new LegacyImportService(_context);

        // Act
        await service.ImportFromStreamAsync(stream);

        // Assert - Nursing
        var nursing = await _context.NursingFeeds.FirstAsync(x => x.ExternalId == "N1");
        Assert.Equal(1700000000.0, nursing.StartTime.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds, 0.01);
        Assert.Equal(1700000600.0, nursing.EndTime!.Value.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds, 0.01);

        // Assert - Bottle
        var bottle = await _context.BottleFeeds.FirstAsync(x => x.ExternalId == "B1");
        Assert.Equal(125, bottle.AmountMl);
        Assert.Equal(BottleContent.Formula, bottle.Content);

        // Assert - Sleep
        var sleep = await _context.SleepSessions.FirstAsync(x => x.ExternalId == "S1");
        Assert.Equal(3000, (sleep.EndTime - sleep.StartTime)!.Value.TotalSeconds);

        // Assert - Diapers
        var d1 = await _context.DiaperChanges.FirstAsync(x => x.ExternalId == "D1");
        Assert.Equal(GajaTrack.Domain.Enums.DiaperType.Wet, d1.Type);
        
        var d2 = await _context.DiaperChanges.FirstAsync(x => x.ExternalId == "D2");
        Assert.Equal(GajaTrack.Domain.Enums.DiaperType.Soiled, d2.Type);

        var d3 = await _context.DiaperChanges.FirstAsync(x => x.ExternalId == "D3");
        Assert.Equal(GajaTrack.Domain.Enums.DiaperType.Mixed, d3.Type);
    }

    public void Dispose()
    {
        _connection.Dispose();
        _context.Dispose();
    }
}
