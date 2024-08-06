using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Localization;
using Project.Web.Shared.Locales.Services;

namespace Project.Web.Shared.Locales.Extensions
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
