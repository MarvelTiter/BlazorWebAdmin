using BlazorWeb.Shared.Components;

namespace BlazorWeb.Shared.Extensions
{
    public static class SharedComponents
    {
        public static IServiceCollection SharedComponentsInit(this IServiceCollection services)
        {
            services.AddScoped<IReconnectorProvider, ReconnectorProvider>();
            return services;
        }
    }
}
