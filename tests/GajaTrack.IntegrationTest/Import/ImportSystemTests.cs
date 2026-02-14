using System.Text;
using GajaTrack.Application.Interfaces;
using GajaTrack.Infrastructure.Persistence;
using GajaTrack.Infrastructure.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;

namespace GajaTrack.IntegrationTest.Import;

public class ImportSystemTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly GajaDbContext _context;
    private readonly BabyPlusImportService _service;

    public ImportSystemTests()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<GajaDbContext>()
            .UseSqlite(_connection)
            .Options;

        _context = new GajaDbContext(options);
        _context.Database.EnsureCreated();

        _service = new BabyPlusImportService(_context, NullLogger<BabyPlusImportService>.Instance);
    }

    [Fact]
    public async Task Import_IosFixture_ShouldSucceed()
    {
        // Arrange
        using var stream = GetFixtureStream("import_ios_sample.json");

        // Act
        var result = await _service.ImportFromStreamAsync(stream);

        // Assert
        Assert.Equal(10, result.NursingFeedsImported);
        Assert.Equal(10, result.BottleFeedsImported);
        Assert.Equal(10, result.SleepSessionsImported);
        Assert.Equal(10, result.DiaperChangesImported);
        Assert.Equal(10, result.CryingSessionsImported);

        // Verify physical persistence
        Assert.Equal(10, await _context.NursingFeeds.CountAsync());
        Assert.Equal(10, await _context.BottleFeeds.CountAsync());
        Assert.Equal(10, await _context.SleepSessions.CountAsync());
        Assert.Equal(10, await _context.DiaperChanges.CountAsync());
        Assert.Equal(10, await _context.CryingSessions.CountAsync());
    }

    [Fact]
    public async Task Import_AndroidFixture_ShouldSucceed()
    {
        // Arrange
        using var stream = GetFixtureStream("import_android_sample.json");

        // Act
        var result = await _service.ImportFromStreamAsync(stream);

        // Assert
        Assert.Equal(10, result.NursingFeedsImported);
        Assert.Equal(10, result.BottleFeedsImported);
        Assert.Equal(10, result.SleepSessionsImported);
        Assert.Equal(10, result.DiaperChangesImported);
        Assert.Equal(10, result.CryingSessionsImported);

        // Verify physical persistence
        Assert.Equal(10, await _context.NursingFeeds.CountAsync());
        Assert.Equal(10, await _context.BottleFeeds.CountAsync());
        Assert.Equal(10, await _context.SleepSessions.CountAsync());
        Assert.Equal(10, await _context.DiaperChanges.CountAsync());
        Assert.Equal(10, await _context.CryingSessions.CountAsync());
    }

    [Fact]
    public async Task Import_Idempotency_ShouldNotCreateDuplicates()
    {
        // Arrange
        using var stream1 = GetFixtureStream("import_ios_sample.json");
        using var stream2 = GetFixtureStream("import_ios_sample.json");

        // Act
        await _service.ImportFromStreamAsync(stream1);
        var result2 = await _service.ImportFromStreamAsync(stream2);

        // Assert
        Assert.Equal(0, result2.NursingFeedsImported);
        Assert.Equal(0, result2.BottleFeedsImported);
        Assert.Equal(0, result2.SleepSessionsImported);
        Assert.Equal(0, result2.DiaperChangesImported);
        Assert.Equal(0, result2.CryingSessionsImported);
        Assert.Equal(10, await _context.NursingFeeds.CountAsync());
    }

    [Fact]
    public async Task Import_UtcPersistence_ShouldBeEnforced()
    {
        // Arrange
        using var stream = GetFixtureStream("import_ios_sample.json");

        // Act
        await _service.ImportFromStreamAsync(stream);

        // Assert
        var feed = await _context.NursingFeeds.FirstOrDefaultAsync(x => x.ExternalId == "1139879339067594");
        Assert.NotNull(feed);
        
        // UtcDateTime record struct should ensure this is UTC
        Assert.Equal(DateTimeKind.Utc, feed.StartTime.Value.Kind);
        
        // 1759921965.1993608 seconds since epoch is 2025-10-08T11:12:45.1993608Z
        var expected = DateTimeOffset.FromUnixTimeSeconds(1759921965).UtcDateTime;
        Assert.Equal(expected.Date, feed.StartTime.Value.Date);
    }

    protected Stream GetFixtureStream(string fileName)
    {
        var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Fixtures", fileName);
        return File.OpenRead(path);
    }

    public void Dispose()
    {
        _connection.Dispose();
        _context.Dispose();
    }
}
