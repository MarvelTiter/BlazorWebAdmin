using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using BlazorTemplate.ClientCore.Locales.EmbeddedJson;
using BlazorTemplate.ClientCore.Locales.Services;

namespace BlazorTemplate.ClientCore.Locales.Extensions;

public static class AddJsonLocalesExtensions
{
    public static IServiceCollection AddJsonLocales(this IServiceCollection services)
    {
        services.TryAddSingleton<IStringLocalizerFactory, EmbeddedJsonLocalizerFactory>();
        services.AddScoped<ILanguageService, LanguageService>();
        services.TryAddTransient(typeof(IStringLocalizer<>), typeof(InteractiveLocalizer<>));
        services.TryAddTransient(typeof(IStringLocalizer), typeof(InteractiveLocalizer<object>));
        return services;
    }
}