using Project.Web.Shared.Components;
using Project.Web.Shared.Pages;
using LightExcel;
using Microsoft.AspNetCore.Components.Authorization;
using Project.AppCore;
using Project.AppCore.Auth;
using Project.AppCore.Locales.Extensions;
using Project.AppCore.Options;
using Project.AppCore.Options.Extensions;

namespace Project.Web.Shared.Extensions
{
    public static class SharedComponents
    {
        public static IServiceCollection SharedComponentsInit(this IServiceCollection services)
        {
            // 多语言服务
            services.AddJsonLocales();
            // excel操作
            services.AddLightExcel();
            //
            services.AutoInjects();
            //
            services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();

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

        public static IServiceCollection ConfigureAppSettings(this IServiceCollection services, WebApplicationBuilder builder)
        {
            services.AddOptions();
            services.Configure<AppSetting>(opt =>
            {
                builder.Configuration.GetSection(nameof(AppSetting)).Bind(opt);
            });
            services.Configure<CultureOptions>(opt =>
            {
                builder.Configuration.GetSection(nameof(CultureOptions)).Bind(opt);
            });
            services.Configure<Token>(opt =>
            {
                builder.Configuration.GetSection(nameof(Token)).Bind(opt);
            });
            return services;
        }
    }
}
