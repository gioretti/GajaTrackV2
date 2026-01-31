using GajaTrack.Application.Interfaces;
using GajaTrack.Domain.Enums;
using GajaTrack.Infrastructure.Services;
using GajaTrack.Infrastructure.Services.ImportHandlers;
using GajaTrack.Domain.Entities;

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
        var newEntries = new List<DiaperChange>();

        // Act
        DiaperChangeImporter.Map(list, [], newEntries);

        // Assert
        Assert.Single(newEntries);
        Assert.Equal(expectedType, newEntries[0].Type);
        Assert.Equal(now, newEntries[0].Time);
    }

    [Fact]
    public void Map_ShouldThrowException_WhenTypeIsUnknown()
    {
        // Arrange
        var jsonItem = new JsonDiaper("pk", DateTime.UtcNow, "UnknownType");
        var list = new List<JsonDiaper> { jsonItem };

        // Act & Assert
        Assert.Throws<ImportValidationException>(() => DiaperChangeImporter.Map(list, [], []));
    }
}