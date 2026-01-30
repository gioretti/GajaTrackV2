using GajaTrack.Application.Interfaces;
using GajaTrack.Infrastructure.Services;
using GajaTrack.Infrastructure.Services.ImportHandlers;

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

        // Act
        var result = SleepSessionImporter.Map(list);

        // Assert
        Assert.Single(result);
        Assert.Equal("pk3", result[0].ExternalId);
        Assert.Equal(now, result[0].StartTime);
        Assert.Equal(jsonItem.EndDate, result[0].EndTime);
    }

    [Fact]
    public void Map_ShouldThrowException_WhenEndDateIsBeforeStartTime()
    {
        // Arrange
        var start = DateTime.UtcNow;
        var end = start.AddMinutes(-10);
        var jsonItem = new JsonSleep("pk_fail", start, end);
        var list = new List<JsonSleep> { jsonItem };

        // Act & Assert
        var ex = Assert.Throws<ImportValidationException>(() => SleepSessionImporter.Map(list));
        Assert.Equal("pk_fail", ex.ExternalId);
        Assert.Contains("before", ex.Message);
    }
}
