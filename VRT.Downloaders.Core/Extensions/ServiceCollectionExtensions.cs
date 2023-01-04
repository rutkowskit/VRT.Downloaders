using Microsoft.Extensions.DependencyInjection;
using System.Runtime.CompilerServices;
using VRT.Assets.Application.Common.Abstractions;
using VRT.Downloaders.Common.Factories;

namespace VRT.Downloaders.Extensions;

public static partial class ServiceCollectionExtensions
{
    public static IServiceCollection WhenNotExists<TService>(this IServiceCollection services,
        Action<IServiceCollection> setupAction)
    {
        foreach (var service in services)
        {
            if (service.ServiceType == typeof(TService))
            {
                return services;
            }
        }
        setupAction(services);
        return services;
    }
    public static IServiceCollection AddTransientWithAbstractFactory<TService, TImplementation>(
        this IServiceCollection services)
        where TService : class
        where TImplementation : class, TService
    {
        services.AddTransient<TService, TImplementation>();
        services.AddSingleton<Func<TService>>(x => () => x.GetRequiredService<TService>());
        services.AddSingleton<IAbstractFactory<TService>, AbstractFactory<TService>>();
        return services;
    }
}
