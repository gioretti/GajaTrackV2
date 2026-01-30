using System.Text.Json;
using System.Text.Json.Serialization;
using GajaTrack.Application.Interfaces;
using GajaTrack.Infrastructure.Persistence;
using GajaTrack.Infrastructure.Services.ImportHandlers;
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

        var nursingFeeds = NursingFeedImporter.Map(data.NursingFeeds);
        dbContext.NursingFeeds.AddRange(nursingFeeds);

        var bottleFeeds = BottleFeedImporter.Map(data.BottleFeeds);
        dbContext.BottleFeeds.AddRange(bottleFeeds);

        var sleepSessions = SleepSessionImporter.Map(data.SleepSessions);
        dbContext.SleepSessions.AddRange(sleepSessions);

        var diaperChanges = DiaperChangeImporter.Map(data.DiaperChanges);
        dbContext.DiaperChanges.AddRange(diaperChanges);

        await dbContext.SaveChangesAsync(cancellationToken);

        return new ImportSummary(
            nursingFeeds.Count,
            bottleFeeds.Count,
            sleepSessions.Count,
            diaperChanges.Count
        );
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
}