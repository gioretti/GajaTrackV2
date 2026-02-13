using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Components.Web;
using GajaTrack.Application.Interfaces;
using GajaTrack.Web.Client.Services;
using GajaTrack.Web.Client.Components;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<Routes>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped<IDailyRhythmMapService, DailyRhythmMapHttpClient>();
builder.Services.AddScoped<IStatsService, StatsHttpClient>();
builder.Services.AddScoped<IExportService, ExportHttpClient>();

await builder.Build().RunAsync();
