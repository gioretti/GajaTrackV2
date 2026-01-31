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

            try
            {
                result.Add(SleepSession.Create(
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
        return result;
    }
}