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
        var service = new BabyPlusImportService(_context);

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
    public async Task Import_ShouldBeIdempotent_AndSkipExistingRecords()
    {
        // Arrange
        var json = """
        {
           "baby_nursingfeed": [{ "pk": "DUP1", "startDate": 1700000000.0, "endDate": 1700000600.0 }]
        }
        """;
        var stream1 = new MemoryStream(Encoding.UTF8.GetBytes(json));
        var stream2 = new MemoryStream(Encoding.UTF8.GetBytes(json));
        var service = new BabyPlusImportService(_context);

        // Act
        var result1 = await service.ImportFromStreamAsync(stream1);
        var result2 = await service.ImportFromStreamAsync(stream2);

        // Assert
        Assert.Equal(1, result1.NursingFeedsImported);
        Assert.Equal(0, result2.NursingFeedsImported); // Should skip the second time
        Assert.Equal(1, await _context.NursingFeeds.CountAsync());
    }

    public void Dispose()
    {
        _connection.Dispose();
        _context.Dispose();
    }
}
