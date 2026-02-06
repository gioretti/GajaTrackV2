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
            var endDate = item.EndDate;
            // Android uses 1970-01-01T00:00:00Z as a placeholder for null EndDate
            if (endDate.HasValue && endDate < item.StartDate && endDate.Value == DateTime.UnixEpoch)
            {
                endDate = null;
            }

            if (item.StartDate > DateTime.UtcNow)
            {
                throw new ImportValidationException(nameof(SleepSession), item.Pk, 
                    $"StartTime {item.StartDate} is in the future.");
            }

            if (existingEntries.TryGetValue(item.Pk, out var existing))
            {
                try
                {
                    existing.Update(UtcDateTime.FromDateTime(item.StartDate), endDate.HasValue ? UtcDateTime.FromDateTime(endDate.Value) : null);
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
                        UtcDateTime.FromDateTime(item.StartDate),
                        endDate.HasValue ? UtcDateTime.FromDateTime(endDate.Value) : null
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
