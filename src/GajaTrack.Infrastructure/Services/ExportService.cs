using System.Text.Json;
using System.Text.Json.Serialization;
using GajaTrack.Application.DTOs.Export;
using GajaTrack.Application.Interfaces;
using GajaTrack.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GajaTrack.Infrastructure.Services;

public class ExportService(GajaDbContext dbContext) : IExportService
{
    public async Task<byte[]> ExportDataAsync(CancellationToken cancellationToken = default)
    {
        var nursing = await dbContext.NursingFeeds.AsNoTracking().ToListAsync(cancellationToken);
        var bottle = await dbContext.BottleFeeds.AsNoTracking().ToListAsync(cancellationToken);
        var sleep = await dbContext.SleepSessions.AsNoTracking().ToListAsync(cancellationToken);
        var diaper = await dbContext.DiaperChanges.AsNoTracking().ToListAsync(cancellationToken);

        var exportData = new GajaTrackExport(
            nursing.Select(x => new ExportNursingFeed(x.Id, DateTime.SpecifyKind(x.StartTime, DateTimeKind.Utc), x.EndTime.HasValue ? DateTime.SpecifyKind(x.EndTime.Value, DateTimeKind.Utc) : null)).ToList(),
            bottle.Select(x => new ExportBottleFeed(x.Id, DateTime.SpecifyKind(x.Time, DateTimeKind.Utc), x.AmountMl, x.Content.ToString())).ToList(),
            sleep.Select(x => new ExportSleepSession(x.Id, DateTime.SpecifyKind(x.StartTime, DateTimeKind.Utc), x.EndTime.HasValue ? DateTime.SpecifyKind(x.EndTime.Value, DateTimeKind.Utc) : null)).ToList(),
            diaper.Select(x => new ExportDiaperChange(x.Id, DateTime.SpecifyKind(x.Time, DateTimeKind.Utc), x.Type.ToString())).ToList()
        );

        var options = new JsonSerializerOptions 
        { 
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase 
        };
        
        var root = new GajaTrackRoot(exportData);

        return JsonSerializer.SerializeToUtf8Bytes(root, options);
    }
}