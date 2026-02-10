using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using GajaTrack.Application.Interfaces;
using GajaTrack.Web.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped<IProtocolService, ProtocolHttpClient>();
builder.Services.AddScoped<IStatsService, StatsHttpClient>();
builder.Services.AddScoped<IExportService, ExportHttpClient>();

await builder.Build().RunAsync();
