using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Localization;
using Project.AppCore.Locales.Services;

namespace Project.AppCore.Locales.Extensions
{
    public static class AddJsonLocalesExtensions
    {
        public static IServiceCollection AddJsonLocales(this IServiceCollection services)
        {
            services.TryAddSingleton<IStringLocalizerFactory, JsonLocalizerFactory>();
            services.TryAddTransient(typeof(IStringLocalizer<>), typeof(StringLocalizer<>));
            services.TryAddTransient(typeof(IStringLocalizer), typeof(StringLocalizer<object>));
            return services;
        }
    }
}
