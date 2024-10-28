using LightExcel;
using LightORM.Providers.Sqlite.Extensions;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using MT.Toolkit.LogTool;
using MT.Toolkit.ReflectionExtension;
using Project.AppCore.Middlewares;
using Project.Constraints;
using Project.Web.Shared;
using Project.Web.Shared.Locales.Extensions;
using System.Data.SQLite;

namespace Project.AppCore
{
    public static class WebService
    {
        public static void AddServerProject(this IHostApplicationBuilder builder, Action<ProjectSetting> action)
        {
            builder.Services.AddClientProject(builder.Configuration, action, out var setting);
            ArgumentNullException.ThrowIfNull(setting.AuthServiceType);
            //var setting = new ProjectSetting();
            //action.Invoke(setting);
            
            builder.Services.AddScoped(typeof(IAuthService), setting.AuthServiceType);
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
            {
                options.SlidingExpiration = true;
            });
            var useProxy = builder.Configuration.GetValue<bool>("AppSetting:UseAspectProxy");
            if (useProxy)
            {
                builder.ConfigureContainer(new AutoAopProxyGenerator.AutoAopProxyServiceProviderFactory());
            }
        }

        public static void UseProject(this IApplicationBuilder app)
        {
            app.UseMiddleware<CheckBrowserEnabledMiddleware>();
            app.UseStaticFiles();
            app.UseMiddleware<RedirectToLauchUrlMiddleware>();
            app.UseWhen(ctx => ctx.Request.Path.StartsWithSegments("/api/download"), a => a.UseMiddleware<FileDownloaderMiddleware>());
            app.UseWhen(ctx => ctx.Request.Path.StartsWithSegments("/ip.client"), a => a.UseMiddleware<GetClientIpMiddleware>());
            app.UseWhen(ctx => ctx.Request.Path.StartsWithSegments("/api")
                && !ctx.Request.Path.StartsWithSegments("/api/download")
                && !ctx.Request.Path.StartsWithSegments("/api/svg")
                && !ctx.Request.Path.StartsWithSegments("/api/file")
                && !ctx.Request.Path.StartsWithSegments("/api/hub")
                && !ctx.Request.Path.StartsWithSegments("/api/account/login")
            , a => a.UseMiddleware<SetUserInfoMiddleware>());
        }
    }
}
