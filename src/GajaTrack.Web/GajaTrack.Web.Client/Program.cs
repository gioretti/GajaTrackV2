using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using GajaTrack.Application.Interfaces;
using GajaTrack.Web.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped<IProtocolService, ProtocolHttpClient>();

await builder.Build().RunAsync();
