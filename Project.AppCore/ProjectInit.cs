﻿using Project.Web.Shared.Components;
using LightExcel;
using Microsoft.AspNetCore.Components.Authorization;
using Project.Web.Shared;
using Project.AppCore.Locales.Extensions;
using Project.AppCore.Auth;
using Project.Constraints;
using Project.Constraints.Options;
using MDbContext;
using AspectCore.Extensions.DependencyInjection;
using Project.AppCore.Middlewares;
using MT.Toolkit.LogTool.LogExtension;
using Microsoft.AspNetCore.DataProtection;
using System.Data.SQLite;
namespace Project.AppCore;

public static class ProjectInit
{
    public static void AddProject(this WebApplicationBuilder builder, Action<ProjectSetting> action)
    {
#if RELEASE
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
#endif
        var services = builder.Services;

        var setting = new ProjectSetting();

        action.Invoke(setting);

        ArgumentNullException.ThrowIfNull(Config.App.Name);

        Config.SetFooter($@"
        <footer style=""text-align:center"">
             <span>{Config.App.Id} ©2023-{DateTime.Now:yyyy} Powered By </span>
             <a href=""#"" target="""" _blank"""">{Config.App.Company}</a>
             <span>{Config.App.Version}</span>
         </footer>
");
        services.AddDataProtection().SetApplicationName(Config.App.Name);
        // 多语言服务
        services.AddJsonLocales();
        // excel操作
        services.AddLightExcel();
        //
        services.AutoInjects(setting.AutoInjectConfig);
        //
        services.AddHttpClient();
        //
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
            builder.AddProjectDbServices(setting);
        }

        if (setting.AddDefaultLogger)
        {
            builder.Logging.AddSimpleLogger(config =>
            {
                config.EnabledLogType = MT.Toolkit.LogTool.LogType.Console | MT.Toolkit.LogTool.LogType.File;
                config.RedirectLogTarget(MT.Toolkit.LogTool.SimpleLogLevel.Information, MT.Toolkit.LogTool.LogType.File);
                config.LogDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");
            });
        }
        var useProxy = builder.Configuration.GetValue<bool>("AppSetting:UseAspectProxy");
        if (useProxy)
        {
            services.ConfigureDynamicProxy();
            builder.Host.UseServiceProviderFactory(new DynamicProxyServiceProviderFactory());
        }

        services.AddSingleton<RedirectToLauchUrlMiddleware>();
        services.AddSingleton<CheckBrowserEnabledMiddleware>();

        Config.AddAssembly(typeof(AppConst).Assembly, typeof(Web.Shared._Imports).Assembly);

        builder.ConfigureAppSettings();
    }

    private static void InterceptorsInit(IServiceCollection services, ProjectSetting setting)
    {
        var settingImplType = setting.SettingProviderType;
        services.AddScoped(settingImplType);
        foreach (var item in setting.interceptorTypes)
        {
            services.AddScoped(typeof(IAddtionalTnterceptor), item);
        }
        services.AddScoped(typeof(ICustomSettingService), provider =>
        {
            var i = provider.GetService(settingImplType) as BasicCustomSetting;
            var interceptors = provider.GetServices(typeof(IAddtionalTnterceptor)) ?? Enumerable.Empty<IAddtionalTnterceptor>();
            foreach (var item in interceptors.Cast<IAddtionalTnterceptor>())
            {
                i!.AddService(item);
            }
            return i;
        });
    }

    public static void AddProjectDbServices(this IHostApplicationBuilder builder, ProjectSetting setting)
    {
        var services = builder.Services;
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

    public static IServiceCollection ConfigureAppSettings(this IHostApplicationBuilder builder)
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
        return services;
    }

    public static void AddDefaultLightOrm(this IHostApplicationBuilder builder, Action<ExpressionSqlOptions>? action = null)
    {
        var connStr = builder.Configuration.GetConnectionString("Sqlite")!;
        builder.Services.AddLightOrm(option =>
        {
            option.SetDatabase(DbBaseType.Sqlite, connStr, SQLiteFactory.Instance).SetWatcher(sql =>
            {
                sql.DbLog = (s,p) =>
                {
#if DEBUG
                    Console.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} Sql => \n{s}\n");
#endif
                };
            });

            action?.Invoke(option);
        });
    }

    public static void UseProject(this WebApplication app)
    {
        app.UseMiddleware<CheckBrowserEnabledMiddleware>();
        app.UseStaticFiles();
        app.UseMiddleware<RedirectToLauchUrlMiddleware>();
        app.MapControllers();
    }
}
