using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Localization;
using Project.Web.Shared.Locales.EmbeddedJson;
using Project.Web.Shared.Locales.Services;

namespace Project.Web.Shared.Locales.Extensions
{
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
}
