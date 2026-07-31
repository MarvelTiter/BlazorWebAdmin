using AutoPageStateContainerGenerator;
using AutoWasmApiGenerator;
using BlazorAdmin;
using BlazorTemplate.AppCore;
using BlazorTemplate.AppCore.Services;
using BlazorTemplate.Constraints;
using BlazorTemplate.Constraints.Services;
using BlazorTemplate.UI.Shared;
using BlazorTemplate.UI.Shared.Pages;
using LightORM;
using LightORM.Providers.Sqlite.Extensions;
using LoggerProviderExtensions;
using MT.LightTask;
var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddHubOptions(option =>
    {
        option.MaximumReceiveMessageSize = 1024 * 1024 * 2;
#if DEBUG
    }).AddInteractiveWebAssemblyComponents();
#else
#if (UseClientProject)
    }).AddInteractiveWebAssemblyComponents();
#endif
#if (!UseClientProject)
    // }).AddInteractiveWebAssemblyComponents();
    });
#endif
#endif
BlazorTemplate.UI.AntBlazor.Extensions.AddAntDesignUI(builder.Services);
//BlazorTemplate.UI.FluentUI.Extensions.AddFluentUI(builder.Services);
//builder.Services.AddAntDesignUI();
//builder.Services.AddFluentUI();
builder.AddServerProject(setting =>
{
    setting.App.Id = "BlazorAdmin";
    setting.App.Name = "BlazorAdmin";
    setting.App.Company = "Marvel";
#if DEBUG
    var appAssembly = typeof(BlazorAdmin.Client._Imports).Assembly;
    setting.ConfigurePage(locator =>
    {
        locator.SetDashboardType<BlazorAdmin.Client.TestPages.TestDashboard>();
        locator.SetUserPageType<TemplateUserPage>();
        locator.SetRunLogPageType<TemplateOperationLog>();
        locator.SetPermissionPageType<TemplatePermissionSetting>();
        locator.SetRolePermissionPageType<TemplateRolePermission>();
    });
#else
#if (UseClientProject)
    var appAssembly  = typeof(BlazorAdmin.Client._Imports).Assembly;
#endif
#if (!UseClientProject)
    var appAssembly = typeof(BlazorAdmin._Imports).Assembly;
#endif
#endif
    AppConst.AppAssembly = appAssembly;
    /*
     * 配置IProjectSettingService和IAuthService(如果需要)
     */

#if DEBUG
    setting.ConfigureSettingProviderType<DevelopSetting>();
    setting.ConfigureAuthService<DevelopAuthenticationService>();
#else
    // setting.ConfigureSettingProviderType<YourSetting>();
    // setting.ConfigureAuthService<YourAuthenticationService>();
#if (ExcludeDefaultPages)
    setting.ConfigureSettingProviderType<BasicSetting>();
    setting.ConfigureAuthService<FreeAuthenticationService>();
#else
    setting.ConfigureSettingProviderType<DevelopSetting>();
    setting.ConfigureAuthService<DevelopAuthenticationService>();
    //set default
    setting.ConfigurePage(locator =>
    {
        locator.SetUserPageType<TemplateUserPage>();
        locator.SetRunLogPageType<TemplateOperationLog>();
        locator.SetPermissionPageType<TemplatePermissionSetting>();
        locator.SetRolePermissionPageType<TemplateRolePermission>();
    });
#endif
#endif
});

//#endif
builder.Logging.AddLocalFileLogger(config =>
{
    config.LogFileSize = 1024 * 1024 * 5;
});

var connStr = builder.Configuration.GetConnectionString("Sqlite")!;
builder.Services.AddLightOrm(option =>
{
    option.UseSqlite(connStr);
    option.SetTableContext<LightOrmTableContext>();
    option.UseInterceptor<LightOrmSqlTrace>();
});

builder.Services.AutoInject();

builder.Services.AddControllers();

builder.Services.AddStateContainers();
#if DEBUG
builder.Services.AddLightTask(o =>
{
    //o.EnableStorage = true;
});
#endif
var app = builder.Build();
// Configure the HTTP request pipeline.
#if DEBUG
app.UseLightTask(c =>
{
    c.AddTask("测试1", (sp, token) =>
    {
        Console.WriteLine($"Task测试1: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        return Task.CompletedTask;
    }, b => b.WithCron("0 */12 * * * ?"));
    c.AddTask<BlazorAdmin.Client.TestPages.Tasks.TestTask>("Task测试2", b => b.WithCron("0 */12 * * * ?"));
});
#endif

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", true);
    app.UseWebAssemblyDebugging();
}
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseProject();
app.UseAntiforgery();
app.MapControllers();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
#if DEBUG
    .AddInteractiveWebAssemblyRenderMode()
#else
#if (UseClientProject)
    .AddInteractiveWebAssemblyRenderMode()
#endif
#if (!UseClientProject)
    //.AddInteractiveWebAssemblyRenderMode()
#endif
#endif
    .AddAdditionalAssemblies([.. AppConst.AdditionalAssemblies]);

app.MapAutoWasmApiEndPoints();

app.Run();