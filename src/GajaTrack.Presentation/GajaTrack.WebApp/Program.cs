using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Components.Web;
using GajaTrack.WebApp.Components;
using GajaTrack.WebApp.Api;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<Routes>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped<DailyRhythmMapApiClient>();
builder.Services.AddScoped<StatsApiClient>();
builder.Services.AddScoped<ExportApiClient>();

await builder.Build().RunAsync();
