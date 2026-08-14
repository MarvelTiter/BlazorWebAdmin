using Microsoft.Extensions.DependencyInjection;

namespace BlazorTemplate.ClientCore.Lockup;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddLookupService(this IServiceCollection services)
    {
        services.AddScoped<ILookupService, LookupService>();
        return services;
    }

    public static IServiceCollection AddStaticLookupProvider<Tp>(this IServiceCollection services)
        where Tp : class, ILookupProvider
    {
        services.AddSingleton<ILookupProvider, Tp>();
        return services;
    }

    public static IServiceCollection AddLookupProvider<Tp>(this IServiceCollection services)
        where Tp : class, ILookupProvider
    {
        services.AddScoped<ILookupProvider, Tp>();
        return services;
    }
}
