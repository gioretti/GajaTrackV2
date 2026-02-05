using GajaTrack.Application.DTOs.Protocol;
using GajaTrack.Application.Interfaces;
using GajaTrack.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GajaTrack.Infrastructure.Services;

public class ProtocolService(GajaDbContext dbContext) : IProtocolService
{
    public async Task<List<ProtocolDay>> GetProtocolAsync(DateOnly startDate, DateOnly endDate, CancellationToken cancellationToken = default)
    {
        // 1. Determine Fetch Range (Buffer to catch overlapping events)
        // We use UTC 06:00 as the definition of the "Day Start" for simplicity in this version.
        var fetchStart = startDate.AddDays(-1).ToDateTime(new TimeOnly(6, 0), DateTimeKind.Utc);
        var fetchEnd = endDate.AddDays(2).ToDateTime(new TimeOnly(6, 0), DateTimeKind.Utc);

        // 2. Fetch Data
        var sleepTask = dbContext.SleepSessions
            .Where(x => x.StartTime < fetchEnd && (x.EndTime == null || x.EndTime > fetchStart))
            .ToListAsync(cancellationToken);
            
        var nursingTask = dbContext.NursingFeeds
            .Where(x => x.StartTime >= fetchStart && x.StartTime < fetchEnd)
            .ToListAsync(cancellationToken);
            
                    var bottleTask = dbContext.BottleFeeds
                    .Where(x => x.Time >= fetchStart && x.Time < fetchEnd)
                    .ToListAsync(cancellationToken);
                    
                var cryingTask = dbContext.CryingSessions
                    .Where(x => x.StartTime < fetchEnd && (x.EndTime == null || x.EndTime > fetchStart))
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
                    var windowStart = d.ToDateTime(new TimeOnly(6, 0), DateTimeKind.Utc);
                    var windowEnd = windowStart.AddDays(1);
                    var dayEvents = new List<ProtocolEvent>();
        
                    // Process Sleep
                    foreach (var s in sleepList)
                    {
                        var sEnd = s.EndTime ?? DateTime.UtcNow; 
                        // Ongoing check: if sEnd is minvalue/null, we treat as now or max? 
                        // The DTO says EndTime nullable. In logic above I used ?? UtcNow.
                        
                        if (s.StartTime < windowEnd && sEnd > windowStart)
                        {
                            var effectiveStart = s.StartTime < windowStart ? windowStart : s.StartTime;
                            var effectiveEnd = sEnd > windowEnd ? windowEnd : sEnd;
                            
                            if (effectiveEnd > effectiveStart)
                            {
                                var startMin = (effectiveStart - windowStart).TotalMinutes;
                                var durMin = (effectiveEnd - effectiveStart).TotalMinutes;
                                dayEvents.Add(new ProtocolEvent(s.Id, ProtocolEventType.Sleep, s.StartTime, startMin, durMin));
                            }
                        }
                    }
                    
                    // Process Crying
                    foreach (var c in cryingList)
                    {
                        var cEnd = c.EndTime ?? DateTime.UtcNow;
                        if (c.StartTime < windowEnd && cEnd > windowStart)
                        {
                            var effectiveStart = c.StartTime < windowStart ? windowStart : c.StartTime;
                            var effectiveEnd = cEnd > windowEnd ? windowEnd : cEnd;
                            
                            if (effectiveEnd > effectiveStart)
                            {
                                var startMin = (effectiveStart - windowStart).TotalMinutes;
                                var durMin = (effectiveEnd - effectiveStart).TotalMinutes;
                                dayEvents.Add(new ProtocolEvent(c.Id, ProtocolEventType.Crying, c.StartTime, startMin, durMin));
                            }
                        }
                    }
        
                    // Process Nursing (Point)
                    foreach (var n in nursingList)
                    {
                        if (n.StartTime >= windowStart && n.StartTime < windowEnd)
                        {
                             var startMin = (n.StartTime - windowStart).TotalMinutes;
                             dayEvents.Add(new ProtocolEvent(n.Id, ProtocolEventType.Nursing, n.StartTime, startMin, 0));
                        }
                    }
                    
                    // Process Bottle (Point)
                    foreach (var b in bottleList)
                    {
                        if (b.Time >= windowStart && b.Time < windowEnd)
                        {
                             var startMin = (b.Time - windowStart).TotalMinutes;
                             var type = b.Content == Domain.Entities.BottleContent.Formula ? ProtocolEventType.BottleFormula : ProtocolEventType.BottleMilk;
                             dayEvents.Add(new ProtocolEvent(b.Id, type, b.Time, startMin, 0, $"{b.AmountMl}ml"));
                        }
                    }
            result.Add(new ProtocolDay(d, windowStart, windowEnd, dayEvents.OrderBy(x => x.StartMinute).ToList()));
        }

        return result;
    }
}