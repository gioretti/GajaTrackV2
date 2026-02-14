using GajaTrack.Application.Interfaces;
using GajaTrack.Infrastructure.Services;
using GajaTrack.Infrastructure.Services.ImportHandlers;
using GajaTrack.Domain.Entities;

namespace GajaTrack.IntegrationTest.Import;

public class BottleFeedImporterTests
{
    [Fact]
    public void Map_ShouldMapJsonToEntity()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var jsonItem = new JsonBottleFeed("pk2", now, 150.5, true);
        var list = new List<JsonBottleFeed> { jsonItem };
        var newEntries = new List<BottleFeed>();

        // Act
        BottleFeedImporter.Map(list, [], newEntries);

        // Assert
        Assert.Single(newEntries);
        Assert.Equal("pk2", newEntries[0].ExternalId);
        Assert.Equal(now, newEntries[0].Time);
        Assert.Equal(150, newEntries[0].AmountMl);
        Assert.Equal(BottleContent.Formula, newEntries[0].Content);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-10)]
    public void Map_ShouldThrowException_WhenAmountIsZeroOrBelow(double amount)
    {
        // Arrange
        var jsonItem = new JsonBottleFeed("pk_fail", DateTime.UtcNow, amount, true);
        var list = new List<JsonBottleFeed> { jsonItem };

        // Act & Assert
        Assert.Throws<ImportValidationException>(() => BottleFeedImporter.Map(list, [], []));
    }
}
