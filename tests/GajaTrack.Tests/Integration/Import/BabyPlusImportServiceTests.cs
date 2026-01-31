using GajaTrack.Infrastructure.Persistence;
using GajaTrack.Infrastructure.Services;
using GajaTrack.Domain.Enums;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace GajaTrack.Tests.Integration.Import;

public class BabyPlusImportServiceTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly GajaDbContext _context;

    public BabyPlusImportServiceTests()
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
    public async Task Import_ShouldProcessFullExportFile()
    {
        // Arrange
        var json = """
        {
           "baby_nursingfeed": [{ "pk": "N1", "startDate": 1700000000.0, "endDate": 1700000600.0 }],
           "baby_bottlefeed": [{ "pk": "B1", "date": 1700001000.0, "amountML": 125, "isFormula": 1 }],
           "baby_sleep": [{ "pk": "S1", "startDate": 1700002000.0, "endDate": 1700005000.0 }],
           "baby_nappy": [{ "pk": "D1", "date": 1700006000.0, "type": "Wet" }]
        }
        """;
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));
        var service = new BabyPlusImportService(_context, Microsoft.Extensions.Logging.Abstractions.NullLogger<BabyPlusImportService>.Instance);

        // Act
        var result = await service.ImportFromStreamAsync(stream);

        // Assert
        Assert.Equal(1, result.NursingFeedsImported);
        Assert.Equal(1, result.BottleFeedsImported);
        Assert.Equal(1, result.SleepSessionsImported);
        Assert.Equal(1, result.DiaperChangesImported);

        Assert.Equal(1, await _context.NursingFeeds.CountAsync());
    }

    [Fact]
    public async Task Import_ShouldBeIdempotent_AndUpdateExistingRecords()
    {
        // Arrange
        var json1 = """
        {
           "baby_nursingfeed": [{ "pk": "DUP1", "startDate": 1700000000.0, "endDate": 1700000600.0 }]
        }
        """;
        var json2 = """
        {
           "baby_nursingfeed": [{ "pk": "DUP1", "startDate": 1700000000.0, "endDate": 1700000999.0 }]
        }
        """;
        var stream1 = new MemoryStream(Encoding.UTF8.GetBytes(json1));
        var stream2 = new MemoryStream(Encoding.UTF8.GetBytes(json2));
        var service = new BabyPlusImportService(_context, Microsoft.Extensions.Logging.Abstractions.NullLogger<BabyPlusImportService>.Instance);

        // Act
        var result1 = await service.ImportFromStreamAsync(stream1);
        var result2 = await service.ImportFromStreamAsync(stream2);

        // Assert
        Assert.Equal(1, result1.NursingFeedsImported);
        Assert.Equal(0, result2.NursingFeedsImported); // 0 NEW feeds, but 1 updated
        
        var feed = await _context.NursingFeeds.FirstAsync(x => x.ExternalId == "DUP1");
        // Verify update happened (EndDate changed from 600 to 999)
        Assert.Equal(1700000999.0, (feed.EndTime!.Value - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds, 0.01);
    }

    [Fact]
    public async Task Import_ShouldHandleNumericPks()
    {
        // Arrange
        var json = """
        {
           "baby_nursingfeed": [{ "pk": 4280847906750135348, "startDate": 1700000000.0, "endDate": 1700000600.0 }]
        }
        """;
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));
        var service = new BabyPlusImportService(_context, Microsoft.Extensions.Logging.Abstractions.NullLogger<BabyPlusImportService>.Instance);

        // Act
        var result = await service.ImportFromStreamAsync(stream);

        // Assert
        Assert.Equal(1, result.NursingFeedsImported);
        var feed = await _context.NursingFeeds.FirstAsync();
        Assert.Equal("4280847906750135348", feed.ExternalId);
    }

    public void Dispose()
    {
        _connection.Dispose();
        _context.Dispose();
    }
}
