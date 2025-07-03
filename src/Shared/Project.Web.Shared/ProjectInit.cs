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
    public static void AddClientProject(this IServiceCollection services, IConfiguration configuration, Action<ProjectSetting> action, out ProjectSetting setting)
    {

        setting = new ProjectSetting();
        setting.ConfigurePage(locator =>
        {
            locator.SetLoginPageType<DefaultLogin>();
        });
#if (ExcludeDefaultService)
#else
        //set default
        setting.ConfigurePage(locator =>
        {
            locator.SetUserPageType<DefaultUserPage>();
            locator.SetRunLogPageType<DefaultOperationLog>();
            locator.SetPermissionPageType<DefaultPermissionSetting>();
            locator.SetRolePermissionPageType<DefaultRolePermission>();
        });
#endif
        action.Invoke(setting);

        services.AddSingleton(setting.locator);
        services.AddScoped(typeof(IProjectSettingService), setting.SettingProviderType);
        services.AddCascadingAuthenticationState();
        ArgumentNullException.ThrowIfNull(AppConst.App.Name);
        AppConst.SetFooter($@"
        <footer style=""text-align:center"">
             <span>{AppConst.App.Id} ©2023-{DateTime.Now:yyyy} Powered By </span>
             <a target="""" _blank"""">{AppConst.App.Company}</a>
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
        //services.AddOptions();
        services.Configure<AppSetting>(configuration.GetSection(nameof(AppSetting)));
        services.Configure<CultureOptions>(configuration.GetSection(nameof(CultureOptions)));
        services.Configure<Token>(configuration.GetSection(nameof(Token)));
    }

    public static T ConfigureOptions<T>(this IServiceCollection services, IConfiguration configuration, string? propertyName = null)
      where T : class, new()
    {
        var name = propertyName ?? typeof(T).Name;
        services.Configure<T>(configuration.GetSection(name));
        var option = new T();
        var section = configuration.GetSection(name);
        section.Bind(option);
        return option;
    }
}
