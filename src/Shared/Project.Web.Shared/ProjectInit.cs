using LightExcel;
using Project.Constraints;
using Project.Constraints.Options;
using Microsoft.Extensions.Hosting;
using MT.Toolkit.LogTool;
using MT.Toolkit.ReflectionExtension;
using Project.Web.Shared.Pages;
using Project.Web.Shared;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Project.Web.Shared.Locales.Extensions;

namespace Project.Web.Shared;
public static class ProjectInit
{
    public static void AddProject(this IServiceCollection services, IConfiguration configuration, Action<ProjectSetting> action, out ProjectSetting setting)
    {
        //try
        //{
        //    var processName = System.Diagnostics.Process.GetCurrentProcess().ProcessName;
        //    bool isNewInstance;
        //    Mutex mtx = new Mutex(true, processName, out isNewInstance);
        //    if (!isNewInstance)
        //    {
        //        var process = System.Diagnostics.Process.GetProcessesByName(processName).FirstOrDefault();
        //        process?.Kill();
        //    }
        //}
        //catch
        //{
        //}


        setting = new ProjectSetting();

        //set default
        setting.ConfigurePage(locator =>
        {
            locator.SetUserPageType<UserPage<User, Power, Role, IStandardUserService, IStandardPermissionService>>();
            locator.SetRunLogPageType<OperationLog<RunLog, IStandardRunLogService>>();
            locator.SetPermissionPageType<PermissionSetting<Power, Role, IStandardPermissionService>>();
            locator.SetRolePermissionPageType<RolePermission<Power, Role, IStandardPermissionService>>();
        });

        action.Invoke(setting);

        services.AddSingleton(setting.locator);
        services.AddScoped(typeof(IProjectSettingService), setting.SettingProviderType);

        ArgumentNullException.ThrowIfNull(AppConst.App.Name);
        AppConst.SetFooter($@"
        <footer style=""text-align:center"">
             <span>{AppConst.App.Id} ©2023-{DateTime.Now:yyyy} Powered By </span>
             <a href=""#"" target="""" _blank"""">{AppConst.App.Company}</a>
             <span>{AppConst.App.Version}</span>
         </footer>
");

        services.AddJsonLocales();
        services.AddLightExcel();
        services.AddScoped(provider =>
        {
            var p = provider.GetService<IDownloadServiceProvider>()!;
            return p.GetService()!;
        });

        AppConst.AddAssembly(typeof(_Imports).Assembly);

        services.ConfigureAppSettings(configuration);
    }

    private static void InterceptorsInit(IServiceCollection services, ProjectSetting setting)
    {
        services.AddScoped(typeof(IProjectSettingService), setting.SettingProviderType);
        foreach (var item in setting.interceptorTypes)
        {
            services.AddScoped(typeof(IAddtionalInterceptor), item);
        }
    }

    internal static void ConfigureAppSettings(this IServiceCollection services, IConfiguration configuration)
    {
        //var services = builder.Services;
        services.AddOptions();
        services.Configure<AppSetting>(opt =>
        {
            configuration.GetSection(nameof(AppSetting)).Bind(opt);
        });
        services.Configure<CultureOptions>(opt =>
        {
            configuration.GetSection(nameof(CultureOptions)).Bind(opt);
        });
        services.Configure<Token>(opt =>
        {
            configuration.GetSection(nameof(Token)).Bind(opt);
        });
    }
}
