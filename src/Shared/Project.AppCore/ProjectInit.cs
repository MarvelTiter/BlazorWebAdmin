using Project.Web.Shared.Components;
using LightExcel;
using Microsoft.AspNetCore.Components.Authorization;
using Project.Web.Shared;
using Project.AppCore.Locales.Extensions;
using Project.AppCore.Auth;
using Project.Constraints;
using Project.Constraints.Options;
using Project.AppCore.Middlewares;
using MT.Toolkit.LogTool.LogExtension;
using Microsoft.AspNetCore.DataProtection;
using System.Data.SQLite;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Builder;
using AspectCore.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Routing;
using MT.Toolkit.LogTool;
using MT.Toolkit.ReflectionExtension;
namespace Project.AppCore;

public static class ProjectInit
{
    public static void AddProject(this IHostApplicationBuilder builder, Action<ProjectSetting> action)
    {
        try
        {
            var processName = System.Diagnostics.Process.GetCurrentProcess().ProcessName;
            bool isNewInstance;
            Mutex mtx = new Mutex(true, processName, out isNewInstance);
            if (!isNewInstance)
            {
                var process = System.Diagnostics.Process.GetProcessesByName(processName).FirstOrDefault();
                process?.Kill();
            }
        }
        catch
        {
        }


        var setting = new ProjectSetting();

        action.Invoke(setting);

        ArgumentNullException.ThrowIfNull(AppConst.App.Name);
        AppConst.SetFooter($@"
        <footer style=""text-align:center"">
             <span>{AppConst.App.Id} ©2023-{DateTime.Now:yyyy} Powered By </span>
             <a href=""#"" target="""" _blank"""">{AppConst.App.Company}</a>
             <span>{AppConst.App.Version}</span>
         </footer>
");

        var services = builder.Services;

        services.AddDataProtection().SetApplicationName(AppConst.App.Name);
        // 多语言服务
        services.AddJsonLocales();
        // excel操作
        services.AddLightExcel();
        //
        services.AutoInjects(setting.AutoInjectConfig);
        //
        services.AddHttpClient();
        //
        services.AddHttpContextAccessor();

        InterceptorsInit(services, setting);

        services.AddControllers().AddApplicationPart(typeof(AppConst).Assembly);
        // 配置 IAuthenticationStateProvider
        services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
        services.AddScoped<IAuthenticationStateProvider>(provider =>
        {
            var auth = provider.GetService<AuthenticationStateProvider>() as CustomAuthenticationStateProvider;
            return auth;
        });

        services.AddScoped<IReconnectorProvider, ReconnectorProvider>();
        services.AddScoped<IDownloadServiceProvider, DownloadServiceProvider>();
        services.AddScoped(provider =>
        {
            var sp = provider.GetService<IDownloadServiceProvider>()!;
            return sp.GetService()!;
        });

        services.AddScoped<IWatermarkServiceFactory, WatermarkServiceFactory>();
        if (setting.AddDefaultProjectServices)
        {
            services.AddProjectDbServices(setting);
        }

        services.AddSingleton<RedirectToLauchUrlMiddleware>();
        services.AddSingleton<CheckBrowserEnabledMiddleware>();

        if (setting.AddFileLogger)
        {
            builder.Logging.AddLocalFileLogger();
        }

        var useProxy = builder.Configuration.GetValue<bool>("AppSetting:UseAspectProxy");
        if (useProxy)
        {
            services.ConfigureDynamicProxy();
            builder.ConfigureContainer(new DynamicProxyServiceProviderFactory());
        }

        AppConst.AddAssembly(typeof(Program).Assembly, typeof(Web.Shared._Imports).Assembly);

        builder.ConfigureAppSettings();
    }

    private static void InterceptorsInit(IServiceCollection services, ProjectSetting setting)
    {
        services.AddSingleton(setting.locator);
        services.AddScoped(typeof(IProjectSettingService), setting.SettingProviderType);
        foreach (var item in setting.interceptorTypes)
        {
            services.AddScoped(typeof(IAddtionalInterceptor), item);
        }
        //services.AddScoped(typeof(IProjectSettingService), provider =>
        //{
        //    var i = provider.GetService(settingImplType) as BasicSetting;
        //    var interceptors = provider.GetServices(typeof(IAddtionalInterceptor)) ?? Enumerable.Empty<IAddtionalInterceptor>();
        //    foreach (var item in interceptors.Cast<IAddtionalInterceptor>())
        //    {
        //        i!.AddService(item);
        //    }
        //    return i;
        //});
    }

    internal static void AddProjectDbServices(this IServiceCollection services, ProjectSetting setting)
    {
        // user
        services.AddScoped(typeof(IUserService<>), typeof(Services.UserService<>));
        services.AddScoped<IUserService>(provider =>
        {
            var srv = provider.GetService(typeof(IUserService<>).MakeGenericType(setting.UserType));
            return srv as IUserService;
        });
        // permission
        services.AddScoped(typeof(IPermissionService<,>).MakeGenericType(setting.PowerType, setting.RoleType), typeof(Services.PemissionService<,,,>).MakeGenericType(setting.PowerType, setting.RoleType, setting.RolePowerType, setting.UserRoleType));
        services.AddScoped<IPermissionService>(provider =>
        {
            var srv = provider.GetService(typeof(IPermissionService<,>).MakeGenericType(setting.PowerType, setting.RoleType));
            return srv as IPermissionService;
        });
        // runlog
        services.AddScoped(typeof(IRunLogService<>), typeof(Services.RunLogService<>));
        services.AddScoped<IRunLogService>(provider =>
        {
            var srv = provider.GetService(typeof(IRunLogService<>).MakeGenericType(setting.RunlogType));
            return srv as IRunLogService;
        });
    }

    internal static void ConfigureAppSettings(this IHostApplicationBuilder builder)
    {
        var services = builder.Services;
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
        if (app is IEndpointRouteBuilder route)
        {
            route.MapControllers();
        }

        var envFunc = app.GetPropertyAccessor<IHostEnvironment>("Environment");
        AppConst.Environment = envFunc.Invoke(app);

        var serFunc = app.GetPropertyAccessor<IServiceProvider>("Services");
        AppConst.Services = serFunc.Invoke(app);
    }
}
