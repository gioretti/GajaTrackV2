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
}
