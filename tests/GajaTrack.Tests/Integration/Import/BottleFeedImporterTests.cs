using GajaTrack.Domain.Enums;
using GajaTrack.Infrastructure.Services;
using GajaTrack.Infrastructure.Services.ImportHandlers;

namespace GajaTrack.Tests.Integration.Import;

public class BottleFeedImporterTests
{
    [Fact]
    public void Map_ShouldMapJsonToEntity()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var jsonItem = new JsonBottleFeed("pk2", now, 150.5, true);
        var list = new List<JsonBottleFeed> { jsonItem };

        // Act
        var result = BottleFeedImporter.Map(list);

        // Assert
        Assert.Single(result);
        Assert.Equal("pk2", result[0].ExternalId);
        Assert.Equal(now, result[0].Time);
        Assert.Equal(150, result[0].AmountMl);
        Assert.Equal(BottleContent.Formula, result[0].Content);
    }
}
