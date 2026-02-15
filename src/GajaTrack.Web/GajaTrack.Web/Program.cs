using GajaTrack.Infrastructure;
using GajaTrack.Application;
using GajaTrack.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContextFactory<GajaTrack.Infrastructure.Persistence.GajaDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped(p => p.GetRequiredService<IDbContextFactory<GajaTrack.Infrastructure.Persistence.GajaDbContext>>().CreateDbContext());

builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration.GetConnectionString("DefaultConnection")!);
builder.Services.AddAntiforgery();

var app = builder.Build();

// Auto-migrate database on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<GajaTrack.Infrastructure.Persistence.GajaDbContext>();
    db.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseBlazorFrameworkFiles();
app.UseStaticFiles();
app.UseAntiforgery();

var api = app.MapGroup("/api");

api.MapGet("/daily-rhythm-map", async (
    [FromQuery] DateOnly startDate, 
    [FromQuery] DateOnly endDate, 
    [FromServices] IDailyRhythmMapService dailyRhythmMapService,
    [FromQuery] bool mostRecentFirst = false, 
    [FromQuery] string? timeZoneId = null) =>
{
    TimeZoneInfo? timeZone = null;
    if (!string.IsNullOrEmpty(timeZoneId))
    {
        try 
        { 
            timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId); 
        }
        catch (TimeZoneNotFoundException) 
        {
            return Results.BadRequest($"Timezone '{timeZoneId}' not found.");
        }
    }

    var result = await dailyRhythmMapService.GetDailyRhythmMapAsync(startDate, endDate, mostRecentFirst, timeZone);
    return Results.Ok(result);
});

api.MapPost("/import/babyplus", async (HttpRequest request, [FromServices] IBabyPlusImportService importService) =>
{
    var summary = await importService.ImportFromStreamAsync(request.Body);
    return Results.Ok(summary);
});

api.MapGet("/export", async ([FromServices] IExportService exportService) =>
{
    var bytes = await exportService.ExportDataAsync();
    return Results.File(bytes, "application/json", $"gajatrack_export_{DateTime.UtcNow:yyyy-MM-dd}.json");
});

api.MapGet("/stats", async ([FromServices] IStatsService statsService) =>
{
    var stats = await statsService.GetStatsAsync();
    return Results.Ok(stats);
});

app.MapFallbackToFile("index.html");

app.Run();

public partial class Program { }
