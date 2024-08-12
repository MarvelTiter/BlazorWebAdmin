using LightExcel;
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
        public static void AddProjectService(this IHostApplicationBuilder builder, Action<ProjectSetting> action)
        {
            builder.Services.AddProject(builder.Configuration, action, out var setting);
            ArgumentNullException.ThrowIfNull(setting.AuthServiceType);
            //var setting = new ProjectSetting();
            //action.Invoke(setting);
            if (setting.AddFileLogger)
            {
                builder.Logging.AddLocalFileLogger();
            }
            builder.Services.AddScoped(typeof(IAuthService), setting.AuthServiceType);
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
            {
                options.SlidingExpiration = true;
            });
        }

        public static void AddDefaultLightOrm(this IHostApplicationBuilder builder, Action<ExpressionSqlOptions>? action = null)
        {
            var connStr = builder.Configuration.GetConnectionString("Sqlite")!;
            builder.Services.AddLightOrm(option =>
            {
                option.SetDatabase(DbBaseType.Sqlite, connStr, SQLiteFactory.Instance).SetWatcher(sql =>
                {
                    sql.DbLog = (s, p) =>
                    {
                        Console.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} Sql => \n{s}\n");
                    };
                });

                action?.Invoke(option);
            });
        }

        public static void UseProject(this IApplicationBuilder app)
        {
            app.UseMiddleware<CheckBrowserEnabledMiddleware>();
            app.UseStaticFiles();
            app.UseMiddleware<RedirectToLauchUrlMiddleware>();
            app.UseWhen(ctx => ctx.Request.Path.StartsWithSegments("/api/download"), a => a.UseMiddleware<FileDownloaderMiddleware>());
            app.UseWhen(ctx => ctx.Request.Path.StartsWithSegments("/ip.client"), a => a.UseMiddleware<GetClientIpMiddleware>());
            app.UseWhen(ctx => ctx.Request.Path.StartsWithSegments("/icons"), a => a.UseMiddleware<IconProviderMiddleware>());
            var envFunc = app.GetPropertyAccessor<IHostEnvironment>("Environment");
            AppConst.Environment = envFunc.Invoke(app);

            var serFunc = app.GetPropertyAccessor<IServiceProvider>("Services");
            AppConst.Services = serFunc.Invoke(app);
        }
    }
}
