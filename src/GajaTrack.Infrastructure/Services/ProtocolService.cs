using GajaTrack.Application.DTOs.Protocol;
using GajaTrack.Application.Interfaces;
using GajaTrack.Domain.Entities;
using GajaTrack.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GajaTrack.Infrastructure.Services;

public class ProtocolService(GajaDbContext dbContext) : IProtocolService
{
    public async Task<List<ProtocolDay>> GetProtocolAsync(DateOnly startDate, DateOnly endDate, bool mostRecentFirst = false, TimeZoneInfo? timeZone = null, CancellationToken cancellationToken = default)
    {
        timeZone ??= TimeZoneInfo.Local;

        // 1. Determine Fetch Range (Buffer to catch overlapping events)
        // Fetch slightly more than needed to ensure we catch events that might cross boundaries after TZ conversion
        var fetchStart = startDate.AddDays(-1).ToDateTime(new TimeOnly(0, 0), DateTimeKind.Utc);
        var fetchEnd = endDate.AddDays(2).ToDateTime(new TimeOnly(0, 0), DateTimeKind.Utc);

        // 2. Fetch Data
        // LINQ to Entities will use the ValueConverter automatically
        var sleepTask = dbContext.SleepSessions
            .Where(x => (DateTime)x.StartTime < fetchEnd && (x.EndTime == null || (DateTime)x.EndTime > fetchStart))
            .ToListAsync(cancellationToken);
            
        var nursingTask = dbContext.NursingFeeds
            .Where(x => (DateTime)x.StartTime >= fetchStart && (DateTime)x.StartTime < fetchEnd)
            .ToListAsync(cancellationToken);
            
        var bottleTask = dbContext.BottleFeeds
            .Where(x => (DateTime)x.Time >= fetchStart && (DateTime)x.Time < fetchEnd)
            .ToListAsync(cancellationToken);
            
        var cryingTask = dbContext.CryingSessions
            .Where(x => (DateTime)x.StartTime < fetchEnd && (x.EndTime == null || (DateTime)x.EndTime > fetchStart))
            .ToListAsync(cancellationToken);

        await Task.WhenAll(sleepTask, nursingTask, bottleTask, cryingTask);

        var result = new List<ProtocolDay>();
        var sleepList = sleepTask.Result;
        var nursingList = nursingTask.Result;
        var bottleList = bottleTask.Result;
        var cryingList = cryingTask.Result;
        
        // 3. Iterate Days
        for (var d = startDate; d <= endDate; d = d.AddDays(1))
        {
            // Window is 06:00 Local to 06:00 Local (Next Day)
            var localStart = new DateTime(d.Year, d.Month, d.Day, 6, 0, 0, DateTimeKind.Unspecified);
            var windowStart = TimeZoneInfo.ConvertTimeToUtc(localStart, timeZone);
            var windowEnd = windowStart.AddDays(1);
            
            var dayEvents = new List<ProtocolEvent>();

            // Process Sleep
            foreach (var s in sleepList)
            {
                var sStart = (DateTime)s.StartTime;
                var sEnd = (DateTime?)(s.EndTime) ?? DateTime.UtcNow; 
                
                if (sStart < windowEnd && sEnd > windowStart)
                {
                    var effectiveStart = sStart < windowStart ? windowStart : sStart;
                    var effectiveEnd = sEnd > windowEnd ? windowEnd : sEnd;
                    
                    if (effectiveEnd > effectiveStart)
                    {
                        var startMin = (effectiveStart - windowStart).TotalMinutes;
                        var durMin = (effectiveEnd - effectiveStart).TotalMinutes;
                        var displayTime = TimeZoneInfo.ConvertTimeFromUtc(sStart, timeZone);
                        dayEvents.Add(new ProtocolEvent(s.Id, ProtocolEventType.Sleep, displayTime, startMin, durMin));
                    }
                }
            }
            
            // Process Crying
            foreach (var c in cryingList)
            {
                var cStart = (DateTime)c.StartTime;
                var cEnd = (DateTime?)(c.EndTime) ?? DateTime.UtcNow;
                if (cStart < windowEnd && cEnd > windowStart)
                {
                    var effectiveStart = cStart < windowStart ? windowStart : cStart;
                    var effectiveEnd = cEnd > windowEnd ? windowEnd : cEnd;
                    
                    if (effectiveEnd > effectiveStart)
                    {
                        var startMin = (effectiveStart - windowStart).TotalMinutes;
                        var durMin = (effectiveEnd - effectiveStart).TotalMinutes;
                        var displayTime = TimeZoneInfo.ConvertTimeFromUtc(cStart, timeZone);
                        dayEvents.Add(new ProtocolEvent(c.Id, ProtocolEventType.Crying, displayTime, startMin, durMin));
                    }
                }
            }

            // Process Nursing (Point)
            foreach (var n in nursingList)
            {
                var nStart = (DateTime)n.StartTime;
                if (nStart >= windowStart && nStart < windowEnd)
                {
                     var startMin = (nStart - windowStart).TotalMinutes;
                     var displayTime = TimeZoneInfo.ConvertTimeFromUtc(nStart, timeZone);
                     dayEvents.Add(new ProtocolEvent(n.Id, ProtocolEventType.Nursing, displayTime, startMin, 0));
                }
            }
            
            // Process Bottle (Point)
            foreach (var b in bottleList)
            {
                var bTime = (DateTime)b.Time;
                if (bTime >= windowStart && bTime < windowEnd)
                {
                     var startMin = (bTime - windowStart).TotalMinutes;
                     var displayTime = TimeZoneInfo.ConvertTimeFromUtc(bTime, timeZone);
                     var type = b.Content == Domain.Entities.BottleContent.Formula ? ProtocolEventType.BottleFormula : ProtocolEventType.BottleMilk;
                     dayEvents.Add(new ProtocolEvent(b.Id, type, displayTime, startMin, 0, $"{b.AmountMl}ml"));
                }
            }
            result.Add(new ProtocolDay(d, windowStart, windowEnd, dayEvents.OrderBy(x => x.StartMinute).ToList()));
        }

        if (mostRecentFirst)
        {
            result.Reverse();
        }

        return result;
    }
}