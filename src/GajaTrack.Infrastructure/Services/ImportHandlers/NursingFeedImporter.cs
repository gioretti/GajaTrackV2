using GajaTrack.Application.Interfaces;
using GajaTrack.Domain.Entities;

namespace GajaTrack.Infrastructure.Services.ImportHandlers;

internal static class NursingFeedImporter
{
    public static void Map(List<JsonNursingFeed>? source, Dictionary<string, NursingFeed> existingEntries, List<NursingFeed> newEntries)
    {
        if (source == null) return;

        foreach (var item in source)
        {
            if (item.StartDate > DateTime.UtcNow)
            {
                throw new ImportValidationException(nameof(NursingFeed), item.Pk, 
                    $"StartTime {item.StartDate} is in the future.");
            }

            if (existingEntries.TryGetValue(item.Pk, out var existing))
            {
                try
                {
                    existing.Update(item.StartDate, item.EndDate);
                }
                catch (ArgumentException ex)
                {
                    throw new ImportValidationException(nameof(NursingFeed), item.Pk, ex.Message);
                }
            }
            else
            {
                try
                {
                    newEntries.Add(NursingFeed.Create(
                        Guid.Empty,
                        item.Pk,
                        item.StartDate,
                        item.EndDate
                    ));
                }
                catch (ArgumentException ex)
                {
                    throw new ImportValidationException(nameof(NursingFeed), item.Pk, ex.Message);
                }
            }
        }
    }
}