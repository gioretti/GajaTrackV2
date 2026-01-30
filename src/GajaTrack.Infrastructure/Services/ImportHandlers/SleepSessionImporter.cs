using GajaTrack.Application.Interfaces;
using GajaTrack.Domain.Entities;

namespace GajaTrack.Infrastructure.Services.ImportHandlers;

internal static class SleepSessionImporter
{
    public static List<SleepSession> Map(List<JsonSleep>? source)
    {
        if (source == null) return [];

        var result = new List<SleepSession>();
        foreach (var item in source)
        {
            if (item.StartDate > DateTime.UtcNow)
            {
                throw new ImportValidationException(nameof(SleepSession), item.Pk, 
                    $"StartTime {item.StartDate} is in the future.");
            }

            if (item.EndDate.HasValue && item.EndDate < item.StartDate)
            {
                throw new ImportValidationException(nameof(SleepSession), item.Pk, 
                    $"EndTime {item.EndDate} is before StartTime {item.StartDate}");
            }

            result.Add(new SleepSession
            {
                BabyId = Guid.Empty,
                ExternalId = item.Pk,
                StartTime = item.StartDate,
                EndTime = item.EndDate
            });
        }
        return result;
    }
}
