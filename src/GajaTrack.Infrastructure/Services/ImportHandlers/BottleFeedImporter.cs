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
            if (item.AmountMl <= 0)
            {
                throw new ImportValidationException(nameof(BottleFeed), item.Pk, 
                    $"Amount {item.AmountMl} must be greater than zero.");
            }

            result.Add(new BottleFeed
            {
                BabyId = Guid.Empty,
                ExternalId = item.Pk,
                Time = item.Date,
                AmountMl = (int)item.AmountMl,
                Content = item.IsFormula ? BottleContent.Formula : BottleContent.BreastMilk
            });
        }
        return result;
    }
}