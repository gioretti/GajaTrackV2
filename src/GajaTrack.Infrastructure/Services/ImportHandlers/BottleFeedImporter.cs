using GajaTrack.Application.Interfaces;
using GajaTrack.Domain.Entities;
using GajaTrack.Domain.Enums;

namespace GajaTrack.Infrastructure.Services.ImportHandlers;

internal static class BottleFeedImporter
{
    public static List<BottleFeed> Map(List<JsonBottleFeed>? source)
    {
        if (source == null) return [];

        var result = new List<BottleFeed>();
        foreach (var item in source)
        {
            if (item.Date > DateTime.UtcNow)
            {
                throw new ImportValidationException(nameof(BottleFeed), item.Pk, 
                    $"Date {item.Date} is in the future.");
            }

            try
            {
                result.Add(BottleFeed.Create(
                    Guid.Empty,
                    item.Pk,
                    item.Date,
                    (int)item.AmountMl,
                    item.IsFormula ? BottleContent.Formula : BottleContent.BreastMilk
                ));
            }
            catch (ArgumentException ex)
            {
                throw new ImportValidationException(nameof(BottleFeed), item.Pk, ex.Message);
            }
        }
        return result;
    }
}
