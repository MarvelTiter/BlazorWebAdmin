using Project.Constraints.Common;

namespace BlazorAdmin;

[AutoInjectGenerator.AutoInjectContext]
public static partial class AutoInjectContext
{
    [AutoInjectGenerator.AutoInjectConfiguration(Include = "SERVER")]
    public static partial void AutoInject(this IServiceCollection services);
    //{
    //    services.AddScoped<Project.Constraints.Store.IAppStore, Project.AppCore.Store.AppStore>();
    //    services.AddScoped<Project.Constraints.IAppSession, Project.AppCore.Store.AppSession>();
    //    services.AddScoped<Project.Constraints.Store.IUserStore, Project.AppCore.Store.UserStore>();
    //    services.AddScoped<Project.Constraints.Services.IRunLogService, Project.AppCore.Services.RunLogService>();
    //    services.AddScoped<Project.Constraints.Services.IStandardRunLogService, Project.AppCore.Services.StandardRunLogService>();
    //    services.AddScoped<Project.Constraints.Services.IPermissionService, Project.AppCore.Services.PermissionService>();
    //    services.AddScoped<Project.Constraints.Services.IStandardPermissionService, Project.AppCore.Services.StandardPermissionService>();
    //    services.AddScoped<Project.Constraints.Services.IUserService, Project.AppCore.Services.UserService>();
    //    services.AddScoped<Project.Constraints.Services.IStandardUserService, Project.AppCore.Services.StandardUserService>();
    //    services.AddSingleton<Project.AppCore.Middlewares.RedirectToLauchUrlMiddleware>();
    //    services.AddSingleton<Project.AppCore.Middlewares.FileDownloaderMiddleware>();
    //    services.AddSingleton<Project.AppCore.Middlewares.GetClientIpMiddleware>();
    //    services.AddSingleton<Project.AppCore.Middlewares.CheckBrowserEnabledMiddleware>();
    //    services.AddScoped<Project.Constraints.Store.IRouterStore, Project.AppCore.Routers.RouterStore>();
    //    services.AddScoped<Project.Constraints.Store.IProtectedLocalStorage, Project.Constraints.Store.MyProtectedLocalStorage>();
    //    services.AddScoped<Project.Constraints.Services.IDownloadServiceProvider, Project.Constraints.Services.DownloadServiceProvider>();
    //    services.AddScoped<Project.Constraints.Services.IWebSettingInitService, Project.Constraints.Services.WebSettingInitService>();
    //    services.AddScoped<Project.Constraints.Services.ILoginService, Project.Services.LoginService>();
    //    services.AddScoped<Project.Constraints.Services.IProjectSettingService, Project.Services.CustomSetting>();
    //    services.AddScoped<Project.Constraints.UI.IUIService, Project.UI.AntBlazor.UIService>();
    //}
}