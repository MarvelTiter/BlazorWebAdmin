
namespace Project.AppCore.Options.Extensions
{
    public static class WebOptionExtensions
    {
        public static IServiceCollection AddWebConfiguration<TOptions>(this IServiceCollection services, Action<TOptions> configureOptions) where TOptions : class
        {
            services.AddOptions();
            services.Configure(configureOptions);
            return services;
        }
    }
}
