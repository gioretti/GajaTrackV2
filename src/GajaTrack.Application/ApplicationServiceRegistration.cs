using GajaTrack.Application.Interfaces;
using GajaTrack.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace GajaTrack.Application;

public static class ApplicationServiceRegistration
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IProtocolService, ProtocolService>();
        return services;
    }
}
