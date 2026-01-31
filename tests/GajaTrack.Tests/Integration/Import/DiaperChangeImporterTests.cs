using GajaTrack.Application.Interfaces;
using GajaTrack.Domain.Enums;
using GajaTrack.Infrastructure.Services;
using GajaTrack.Infrastructure.Services.ImportHandlers;

namespace GajaTrack.Tests.Integration.Import;

public class DiaperChangeImporterTests
{
    [Theory]
    [InlineData("Wet", DiaperType.Wet)]
    [InlineData("Soiled", DiaperType.Soiled)]
    [InlineData("Mixed", DiaperType.Mixed)]
    public void Map_ShouldMapTypesCorrectly(string sourceType, DiaperType expectedType)
    {
        // Arrange
        var now = DateTime.UtcNow;
        var jsonItem = new JsonDiaper("pk", now, sourceType);
        var list = new List<JsonDiaper> { jsonItem };

        // Act
        var result = DiaperChangeImporter.Map(list);

        // Assert
        Assert.Single(result);
        Assert.Equal(expectedType, result[0].Type);
        Assert.Equal(now, result[0].Time);
    }

    [Fact]
    public void Map_ShouldThrowException_WhenTypeIsUnknown()
    {
        // Arrange
        var jsonItem = new JsonDiaper("pk", DateTime.UtcNow, "UnknownType");
        var list = new List<JsonDiaper> { jsonItem };

        // Act & Assert
        Assert.Throws<ImportValidationException>(() => DiaperChangeImporter.Map(list));
    }
}
