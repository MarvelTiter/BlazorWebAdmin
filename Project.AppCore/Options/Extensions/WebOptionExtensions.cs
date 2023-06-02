using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Project.AppCore.Options.Extensions
{
    public static class WebOptionExtensions
    {
        public static IServiceCollection AddWebConfiguration<TOptions>(this IServiceCollection services, Action<TOptions> configureOptions = null) where TOptions : class
        {
            services.AddOptions();
            configureOptions ??= op => { };
            services.Configure(configureOptions);
            return services;
        }
    }
}
