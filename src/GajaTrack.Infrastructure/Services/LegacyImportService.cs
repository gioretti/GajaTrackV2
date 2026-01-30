using System.Text.Json;
using System.Text.Json.Serialization;
using GajaTrack.Application.Interfaces;
using GajaTrack.Domain.Entities;
using GajaTrack.Domain.Enums;
using GajaTrack.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GajaTrack.Infrastructure.Services;

public class LegacyImportService(GajaDbContext dbContext) : ILegacyImportService
{
    public async Task<ImportSummary> ImportFromStreamAsync(Stream stream, CancellationToken cancellationToken = default)
    {
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        options.Converters.Add(new PolymorphicDateTimeConverter());
        options.Converters.Add(new PolymorphicBoolConverter());
        
        var data = await JsonSerializer.DeserializeAsync<BabyPlusExport>(stream, options, cancellationToken);
        
        if (data is null) return new ImportSummary(0, 0, 0, 0);

        var nursingFeeds = MapNursingFeeds(data.NursingFeeds);
        dbContext.NursingFeeds.AddRange(nursingFeeds);

        var bottleFeeds = MapBottleFeeds(data.BottleFeeds);
        dbContext.BottleFeeds.AddRange(bottleFeeds);

        var sleepSessions = MapSleepSessions(data.SleepSessions);
        dbContext.SleepSessions.AddRange(sleepSessions);

        var diaperChanges = MapDiaperChanges(data.DiaperChanges);
        dbContext.DiaperChanges.AddRange(diaperChanges);

        await dbContext.SaveChangesAsync(cancellationToken);

        return new ImportSummary(
            nursingFeeds.Count,
            bottleFeeds.Count,
            sleepSessions.Count,
            diaperChanges.Count
        );
    }

    private List<NursingFeed> MapNursingFeeds(List<JsonNursingFeed>? source)
    {
        var result = new List<NursingFeed>();
        if (source == null) return result;

        foreach (var item in source)
        {
            if (item.Pk == null) continue;
            result.Add(new NursingFeed
            {
                BabyId = Guid.Empty,
                ExternalId = item.Pk,
                StartTime = item.StartDate,
                EndTime = item.EndDate.Year == 1 ? null : item.EndDate
            });
        }
        return result;
    }

    private List<BottleFeed> MapBottleFeeds(List<JsonBottleFeed>? source)
    {
        var result = new List<BottleFeed>();
        if (source == null) return result;

        foreach (var item in source)
        {
            if (item.Pk == null) continue;
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

    private List<SleepSession> MapSleepSessions(List<JsonSleep>? source)
    {
        var result = new List<SleepSession>();
        if (source == null) return result;

        foreach (var item in source)
        {
            if (item.Pk == null) continue;
            result.Add(new SleepSession
            {
                BabyId = Guid.Empty,
                ExternalId = item.Pk,
                StartTime = item.StartDate,
                EndTime = item.EndDate.Year == 1 ? null : item.EndDate
            });
        }
        return result;
    }

    private List<DiaperChange> MapDiaperChanges(List<JsonDiaper>? source)
    {
        var result = new List<DiaperChange>();
        if (source == null) return result;

        foreach (var item in source)
        {
            if (item.Pk == null) continue;
            
            DiaperType type = DiaperType.Wet; // Default
            if (item.Type != null)
            {
                if (item.Type.Contains("Soiled", StringComparison.OrdinalIgnoreCase)) type = DiaperType.Soiled;
                else if (item.Type.Contains("Mixed", StringComparison.OrdinalIgnoreCase)) type = DiaperType.Mixed;
            }

            result.Add(new DiaperChange
            {
                BabyId = Guid.Empty,
                ExternalId = item.Pk,
                Time = item.Date,
                Type = type
            });
        }
        return result;
    }

    private class PolymorphicDateTimeConverter : JsonConverter<DateTime>
    {
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Number)
            {
                // Unix Timestamp (double)
                double unixTime = reader.GetDouble();
                DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                return dateTime.AddSeconds(unixTime);
            }
            else if (reader.TokenType == JsonTokenType.String)
            {
                // ISO 8601 String
                if (DateTime.TryParse(reader.GetString(), 
                    System.Globalization.CultureInfo.InvariantCulture, 
                    System.Globalization.DateTimeStyles.AdjustToUniversal, 
                    out var date))
                {
                    return date;
                }
            }
            return default;
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString("o"));
        }
    }

    private class PolymorphicBoolConverter : JsonConverter<bool>
    {
        public override bool Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.True) return true;
            if (reader.TokenType == JsonTokenType.False) return false;
            if (reader.TokenType == JsonTokenType.Number)
            {
                return reader.GetInt32() == 1;
            }
            return false;
        }

        public override void Write(Utf8JsonWriter writer, bool value, JsonSerializerOptions options)
        {
            writer.WriteBooleanValue(value);
        }
    }

    private class BabyPlusExport
    {
        [JsonPropertyName("baby_nursingfeed")]
        public List<JsonNursingFeed>? NursingFeeds { get; set; }

        [JsonPropertyName("baby_bottlefeed")]
        public List<JsonBottleFeed>? BottleFeeds { get; set; }
        
        [JsonPropertyName("baby_sleep")]
        public List<JsonSleep>? SleepSessions { get; set; }
        
        [JsonPropertyName("baby_nappy")]
        public List<JsonDiaper>? DiaperChanges { get; set; }
    }
    
    private class JsonNursingFeed
    {
        public string? Pk { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; } 
    }

    private class JsonBottleFeed
    {
        public string? Pk { get; set; }
        public DateTime Date { get; set; }
        public double AmountMl { get; set; }
        public bool IsFormula { get; set; }
    }

    private class JsonSleep
    {
        public string? Pk { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
    
    private class JsonDiaper
    {
        public string? Pk { get; set; }
        public DateTime Date { get; set; }
        public string? Type { get; set; }
    }
}
