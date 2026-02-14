using GajaTrack.Application.Interfaces;
using GajaTrack.Infrastructure.Services;
using GajaTrack.Infrastructure.Services.ImportHandlers;
using GajaTrack.Domain.Entities;

namespace GajaTrack.IntegrationTest.Import;

public class NursingFeedImporterTests
{
    [Fact]
    public void Map_ShouldMapJsonToEntity()
    {
        // Arrange
        var jsonItem = new JsonNursingFeed("pk1", DateTime.UtcNow, DateTime.UtcNow.AddMinutes(10));
        var list = new List<JsonNursingFeed> { jsonItem };
        var newEntries = new List<NursingFeed>();

        // Act
        NursingFeedImporter.Map(list, [], newEntries);

        // Assert
        Assert.Single(newEntries);
        Assert.Equal("pk1", newEntries[0].ExternalId);
        Assert.Equal(jsonItem.StartDate, newEntries[0].StartTime);
        Assert.Equal(jsonItem.EndDate, newEntries[0].EndTime);
    }

    [Fact]
    public void Map_ShouldThrowException_WhenEndDateIsBeforeStartTime()
    {
        // Arrange
        var start = DateTime.UtcNow;
        var end = start.AddMinutes(-10);
        var jsonItem = new JsonNursingFeed("pk_fail", start, end);
        var list = new List<JsonNursingFeed> { jsonItem };

        // Act & Assert
        Assert.Throws<ImportValidationException>(() => NursingFeedImporter.Map(list, [], []));
    }

    [Fact]
    public void Map_ShouldHandleZeroEndDate_BySettingToNull()
    {
        // Arrange
        var start = DateTime.UtcNow;
        var jsonItem = new JsonNursingFeed("pk_zero", start, null);
        var list = new List<JsonNursingFeed> { jsonItem };
        var newEntries = new List<NursingFeed>();

        // Act
        NursingFeedImporter.Map(list, [], newEntries);

        // Assert
        Assert.Null(newEntries[0].EndTime);
    }

    [Fact]
    public void Map_ShouldHandleEpochEndDate_BySettingToNull_WhenBeforeStartTime()
    {
        // Arrange
        var start = new DateTime(2025, 1, 1, 12, 0, 0, DateTimeKind.Utc);
        var epoch = DateTimeOffset.FromUnixTimeSeconds(0).UtcDateTime; // 1970-01-01T00:00:00Z
        var jsonItem = new JsonNursingFeed("pk_epoch", start, epoch);
        var list = new List<JsonNursingFeed> { jsonItem };
        var newEntries = new List<NursingFeed>();

        // Act
        NursingFeedImporter.Map(list, [], newEntries);

        // Assert
        Assert.Null(newEntries[0].EndTime);
    }
}
