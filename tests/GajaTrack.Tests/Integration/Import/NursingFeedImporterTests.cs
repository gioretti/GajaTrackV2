using GajaTrack.Application.Interfaces;
using GajaTrack.Infrastructure.Services;
using GajaTrack.Infrastructure.Services.ImportHandlers;

namespace GajaTrack.Tests.Integration.Import;

public class NursingFeedImporterTests
{
    [Fact]
    public void Map_ShouldMapJsonToEntity()
    {
        // Arrange
        var jsonItem = new JsonNursingFeed("pk1", DateTime.UtcNow, DateTime.UtcNow.AddMinutes(10));
        var list = new List<JsonNursingFeed> { jsonItem };

        // Act
        var result = NursingFeedImporter.Map(list);

        // Assert
        Assert.Single(result);
        Assert.Equal("pk1", result[0].ExternalId);
        Assert.Equal(jsonItem.StartDate, result[0].StartTime);
        Assert.Equal(jsonItem.EndDate, result[0].EndTime);
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
        var ex = Assert.Throws<ImportValidationException>(() => NursingFeedImporter.Map(list));
        Assert.Equal("pk_fail", ex.ExternalId);
        Assert.Contains("before", ex.Message);
    }

    [Fact]
    public void Map_ShouldHandleZeroEndDate_BySettingToNull()
    {
        // Arrange
        var start = DateTime.UtcNow;
        // The converter is responsible for returning null if JSON value is 0.
        // We simulate that by passing null here.
        var jsonItem = new JsonNursingFeed("pk_zero", start, null);
        var list = new List<JsonNursingFeed> { jsonItem };

        // Act
        var result = NursingFeedImporter.Map(list);

        // Assert
        Assert.Null(result[0].EndTime);
    }
}
