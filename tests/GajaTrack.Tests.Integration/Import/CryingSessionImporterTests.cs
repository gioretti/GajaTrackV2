using GajaTrack.Application.Interfaces;
using GajaTrack.Infrastructure.Services;
using GajaTrack.Infrastructure.Services.ImportHandlers;
using GajaTrack.Domain.Entities;

namespace GajaTrack.Tests.Integration.Import;

public class CryingSessionImporterTests
{
    [Fact]
    public void Map_ShouldMapJsonToEntity()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var jsonItem = new JsonCryingSession("pk3", now, now.AddMinutes(15));
        var list = new List<JsonCryingSession> { jsonItem };
        var newEntries = new List<CryingSession>();

        // Act
        CryingSessionImporter.Map(list, [], newEntries);

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
        var jsonItem = new JsonCryingSession("pk_fail", start, end);
        var list = new List<JsonCryingSession> { jsonItem };

        // Act & Assert
        Assert.Throws<ImportValidationException>(() => CryingSessionImporter.Map(list, [], []));
    }

    [Fact]
    public void Map_ShouldHandleEpochEndDate_BySettingToNull()
    {
        // Arrange
        var start = new DateTime(2025, 1, 1, 12, 0, 0, DateTimeKind.Utc);
        var epoch = DateTimeOffset.FromUnixTimeSeconds(0).UtcDateTime; // 1970-01-01T00:00:00Z
        var jsonItem = new JsonCryingSession("pk_epoch", start, epoch);
        var list = new List<JsonCryingSession> { jsonItem };
        var newEntries = new List<CryingSession>();

        // Act
        CryingSessionImporter.Map(list, [], newEntries);

        // Assert
        Assert.Null(newEntries[0].EndTime);
        Assert.Null(newEntries[0].Duration);
    }

    [Fact]
    public void Map_ShouldUpdateExistingEntity()
    {
        // Arrange
        var start = UtcDateTime.FromDateTime(DateTime.UtcNow);
        var initialEnd = UtcDateTime.FromDateTime(start.Value.AddMinutes(10));
        var existing = CryingSession.Create(Guid.NewGuid(), "pk_update", start, initialEnd);
        var dictionary = new Dictionary<string, CryingSession> { { "pk_update", existing } };
        
        var newEnd = start.Value.AddMinutes(20);
        var jsonItem = new JsonCryingSession("pk_update", start, newEnd);
        var list = new List<JsonCryingSession> { jsonItem };
        var newEntries = new List<CryingSession>();

        // Act
        CryingSessionImporter.Map(list, dictionary, newEntries);

        // Assert
        Assert.Empty(newEntries); // Should not add new
        Assert.Equal(newEnd, (DateTime?)existing.EndTime);
    }
}
