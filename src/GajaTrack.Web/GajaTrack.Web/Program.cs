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
    [FromQuery] bool mostRecentFirst, 
    [FromQuery] string? timeZoneId,
    [FromServices] IDailyRhythmMapService service) =>
{
    TimeZoneInfo? timeZone = null;
    if (!string.IsNullOrEmpty(timeZoneId))
    {
        try { timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId); }
        catch (TimeZoneNotFoundException) { /* Fallback to Local/UTC */ }
    }

    var result = await service.GetDailyRhythmMapAsync(startDate, endDate, mostRecentFirst, timeZone);
    return Results.Ok(result);
});

api.MapPost("/import/babyplus", async (HttpRequest request, [FromServices] IBabyPlusImportService service) =>
{
    var summary = await service.ImportFromStreamAsync(request.Body);
    return Results.Ok(summary);
});

api.MapGet("/export", async ([FromServices] IExportService service) =>
{
    var bytes = await service.ExportDataAsync();
    return Results.File(bytes, "application/json", $"gajatrack_export_{DateTime.UtcNow:yyyy-MM-dd}.json");
});

api.MapGet("/stats", async ([FromServices] IStatsService service) =>
{
    var stats = await service.GetStatsAsync();
    return Results.Ok(stats);
});

app.MapFallbackToFile("index.html");

app.Run();

public partial class Program { }
