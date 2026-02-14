using GajaTrack.Application.Interfaces;
using GajaTrack.Infrastructure.Services;
using GajaTrack.Infrastructure.Services.ImportHandlers;
using GajaTrack.Domain.Entities;

namespace GajaTrack.Tests.Integration.Import;

public class SleepSessionImporterTests
{
    [Fact]
    public void Map_ShouldMapJsonToEntity()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var jsonItem = new JsonSleep("pk3", now, now.AddHours(2));
        var list = new List<JsonSleep> { jsonItem };
        var newEntries = new List<SleepSession>();

        // Act
        SleepSessionImporter.Map(list, [], newEntries);

        // Assert
        Assert.Single(newEntries);
        Assert.Equal("pk3", newEntries[0].ExternalId);
        Assert.Equal(now, newEntries[0].StartTime);
        Assert.Equal(jsonItem.EndDate, newEntries[0].EndTime);
    }

    [Fact]
    public void Map_ShouldThrowException_WhenEndDateIsBeforeStartTime()
    {
        // Arrange
        var start = DateTime.UtcNow;
        var end = start.AddMinutes(-30);
        var jsonItem = new JsonSleep("pk_fail", start, end);
        var list = new List<JsonSleep> { jsonItem };

        // Act & Assert
        Assert.Throws<ImportValidationException>(() => SleepSessionImporter.Map(list, [], []));
    }

    [Fact]
    public void Map_ShouldHandleEpochEndDate_BySettingToNull_WhenBeforeStartTime()
    {
        // Arrange
        var start = new DateTime(2025, 1, 1, 12, 0, 0, DateTimeKind.Utc);
        var epoch = DateTimeOffset.FromUnixTimeSeconds(0).UtcDateTime; // 1970-01-01T00:00:00Z
        var jsonItem = new JsonSleep("pk_epoch", start, epoch);
        var list = new List<JsonSleep> { jsonItem };
        var newEntries = new List<SleepSession>();

        // Act
        SleepSessionImporter.Map(list, [], newEntries);

        // Assert
        Assert.Null(newEntries[0].EndTime);
    }
}