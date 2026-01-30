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
}
