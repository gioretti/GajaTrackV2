using GajaTrack.Application.Interfaces;
using GajaTrack.Domain.Entities;

namespace GajaTrack.Infrastructure.Services.ImportHandlers;

internal static class BottleFeedImporter
{
    public static void Map(List<JsonBottleFeed>? source, Dictionary<string, BottleFeed> existingEntries, List<BottleFeed> newEntries)
    {
        if (source == null) return;

        foreach (var item in source)
        {
            if (item.Date > DateTime.UtcNow)
            {
                throw new ImportValidationException(nameof(BottleFeed), item.Pk, 
                    $"Date {item.Date} is in the future.");
            }

            var content = item.IsFormula ? BottleContent.Formula : BottleContent.BreastMilk;

            if (existingEntries.TryGetValue(item.Pk, out var existing))
            {
                try
                {
                    existing.Update(item.Date, (int)item.AmountMl, content);
                }
                catch (ArgumentException ex)
                {
                    throw new ImportValidationException(nameof(BottleFeed), item.Pk, ex.Message);
                }
            }
            else
            {
                try
                {
                    newEntries.Add(BottleFeed.Create(
                        Guid.Empty,
                        item.Pk,
                        item.Date,
                        (int)item.AmountMl,
                        content
                    ));
                }
                catch (ArgumentException ex)
                {
                    throw new ImportValidationException(nameof(BottleFeed), item.Pk, ex.Message);
                }
            }
        }
    }
}