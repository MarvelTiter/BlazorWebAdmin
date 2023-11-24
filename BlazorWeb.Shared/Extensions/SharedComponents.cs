using BlazorWeb.Shared.Components;
using BlazorWeb.Shared.Pages;

namespace BlazorWeb.Shared.Extensions
{
    public static class SharedComponents
    {
        public static IServiceCollection SharedComponentsInit(this IServiceCollection services)
        {
            services.AddScoped<IReconnectorProvider, ReconnectorProvider>();
            services.AddScoped<IDownloadServiceProvider, DownloadServiceProvider>();
            services.AddSingleton<IDashboardContentProvider, DashboardContentProvider>();
            services.AddScoped<IDownloadService>(provider =>
            {
                var sp = provider.GetService<IDownloadServiceProvider>();
                return sp?.GetService();
            });
            return services;
        }
    }
}
