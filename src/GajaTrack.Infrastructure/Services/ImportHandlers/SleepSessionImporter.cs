using GajaTrack.Application.Interfaces;
using GajaTrack.Domain.Entities;

namespace GajaTrack.Infrastructure.Services.ImportHandlers;

internal static class SleepSessionImporter
{
    public static void Map(List<JsonSleep>? source, Dictionary<string, SleepSession> existingEntries, List<SleepSession> newEntries)
    {
        if (source == null) return;

        foreach (var item in source)
        {
            if (item.StartDate > DateTime.UtcNow)
            {
                throw new ImportValidationException(nameof(SleepSession), item.Pk, 
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
                    throw new ImportValidationException(nameof(SleepSession), item.Pk, ex.Message);
                }
            }
            else
            {
                try
                {
                    newEntries.Add(SleepSession.Create(
                        Guid.Empty,
                        item.Pk,
                        item.StartDate,
                        item.EndDate
                    ));
                }
                catch (ArgumentException ex)
                {
                    throw new ImportValidationException(nameof(SleepSession), item.Pk, ex.Message);
                }
            }
        }
    }
}
