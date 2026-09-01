using Microsoft.Extensions.DependencyInjection;

namespace BlazorTemplate.ClientCore.Lockup;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddLookupService(this IServiceCollection services, Action<LookupServiceBuilder>? configure = null)
    {
        services.AddScoped<ILookupService, LookupService>();
        var builder = new LookupServiceBuilder(services);
        configure?.Invoke(builder);
        return services;
    }

    //public static IServiceCollection AddStaticLookupProvider<Tp>(this IServiceCollection services)
    //    where Tp : class, ILookupProvider
    //{
    //    services.AddSingleton<ILookupProvider, Tp>();
    //    return services;
    //}

    //public static IServiceCollection AddLookupProvider<Tp>(this IServiceCollection services)
    //    where Tp : class, ILookupProvider
    //{
    //    services.AddScoped<ILookupProvider, Tp>();
    //    return services;
    //}

    //public static IServiceCollection AddEnumLookupProvider<TEnum>(this IServiceCollection services)
    //    where TEnum : struct, Enum
    //{
    //    services.AddSingleton<ILookupProvider, EnumLookupProvider>();
    //    return services;
    //}
}
