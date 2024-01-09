using Project.Web.Shared.Components;
using Project.Web.Shared.Pages;
using LightExcel;
using Microsoft.AspNetCore.Components.Authorization;
using Project.Web.Shared;
using Project.AppCore.Locales.Extensions;
using Project.AppCore.Auth;
using Project.Constraints;
using Project.Constraints.Options;
using Project.UI.AntBlazor;
using MDbContext;
using Microsoft.Data.Sqlite;
using AspectCore.Extensions.DependencyInjection;
using Project.AppCore.Middlewares;
using MT.Toolkit.LogTool.LogExtension;
using Microsoft.AspNetCore.DataProtection;
using System.Reflection;
namespace Project.AppCore;

public class ProjectSetting
{
    public Type SettingProviderType { get; set; }
    public Action<AutoInjectFilter>? AutoInjectConfig { get; set; }
    public bool AddDefaultLogger { get; set; }
    public AppInfo App => Config.App;
}

public static class SharedComponents
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

        Config.SetFooter($@"
        <footer style=""text-align:center"">
             <span>{Config.App.Id} ©2023-{DateTime.Now:yyyy} Powered By </span>
             <a href=""#"" target="""" _blank"""">{Config.App.Company}</a>
             <span>{Config.App.Version}</span>
         </footer>
");

        var name = Assembly.GetEntryAssembly()?.GetName().Name!;
        services.AddDataProtection().SetApplicationName(name);
        //
        services.AddAntDesignUI();
        // 多语言服务
        services.AddJsonLocales();
        // excel操作
        services.AddLightExcel();
        //
        services.AutoInjects(setting.AutoInjectConfig);
        //
        services.AddHttpClient();
        //
        var settingImplType = setting.SettingProviderType;
        services.AddScoped(typeof(ICustomSettingProvider), settingImplType);
        services.AddControllers().AddApplicationPart(typeof(AppConst).Assembly);
        services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
        services.AddScoped<IAuthenticationStateProvider>(provider =>
        {
            var auth = provider.GetService<AuthenticationStateProvider>() as CustomAuthenticationStateProvider;
            return auth;
        });

        services.AddScoped<IReconnectorProvider, ReconnectorProvider>();
        services.AddScoped<IDownloadServiceProvider, DownloadServiceProvider>();
        services.AddSingleton<IDashboardContentProvider, DashboardContentProvider>();
        services.AddScoped<IDownloadService>(provider =>
        {
            var sp = provider.GetService<IDownloadServiceProvider>();
            return sp?.GetService();
        });

        if (setting.AddDefaultLogger)
        {
            builder.Logging.AddSimpleLogger(config =>
            {
                config.EnabledLogType = MT.Toolkit.LogTool.LogType.Console | MT.Toolkit.LogTool.LogType.File;
                config.RedirectLogTarget(MT.Toolkit.LogTool.SimpleLogLevel.Information, MT.Toolkit.LogTool.LogType.File);
                config.LogDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");
            });
        }


        services.ConfigureDynamicProxy();
        builder.Host.UseServiceProviderFactory(new DynamicProxyServiceProviderFactory());

        services.AddSingleton<RedirectToLauchUrlMiddleware>();


        Config.AddAssembly(typeof(AppConst).Assembly);

        builder.ConfigureAppSettings();
    }

    public static IServiceCollection ConfigureAppSettings(this WebApplicationBuilder builder)
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

    public static void AddDefaultLightOrm(this WebApplicationBuilder builder)
    {

        builder.Services.AddLightOrm(option =>
            option.SetDatabase(DbBaseType.Sqlite, () =>
            {
                var connStr = builder.Configuration.GetConnectionString("Sqlite");
                return new SqliteConnection(connStr);
            })
    .SetWatcher(sql =>
    {
        sql.BeforeExecute = e =>
        {
#if DEBUG
            Console.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} Sql => \n{e.Sql}\n");
#endif
        };
    })
            );
    }

    public static void UseProject(this WebApplication app)
    {
        app.UseMiddleware<RedirectToLauchUrlMiddleware>();

        app.MapControllers();
    }
}
