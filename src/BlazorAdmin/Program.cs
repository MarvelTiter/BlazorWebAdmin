using AutoPageStateContainerGenerator;
using BlazorAdmin;
using LightORM;
using LightORM.Providers.Sqlite.Extensions;
using LoggerProviderExtensions;
using Project.AppCore;
using Project.AppCore.Services;
using Project.Constraints;
using Project.Web.Shared;
using MT.LightTask;
var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddHubOptions(option =>
    {
        option.MaximumReceiveMessageSize = 1024 * 1024 * 2;
    })
    .AddInteractiveWebAssemblyComponents();

Project.UI.AntBlazor.Extensions.AddAntDesignUI(builder.Services);
//Project.UI.FluentUI.Extensions.AddFluentUI(builder.Services);
//builder.Services.AddAntDesignUI();
//builder.Services.AddFluentUI();
builder.AddServerProject(setting =>
{
    setting.App.Id = "Test";
    setting.App.Name = "Demo";
    setting.App.Company = "Marvel";
#if DEBUG
    var appAssembly = typeof(BlazorAdmin.Client._Imports).Assembly;
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
    setting.ConfigureSettingProviderType<CustomSetting>();
    setting.ConfigureAuthService<BlazorAdminAuthenticationService>();
#endif
#if (ExcludeDefaultService)
    // setting.ConfigureSettingProviderType<YourSetting>();
    // setting.ConfigureSettingProviderType<BasicSetting>();
    // setting.ConfigureAuthService<YourAuthenticationService>();
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
builder.Services.AddLightTask();
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
    }, b => b.WithCron("*/12 * * * * ?").Build());
    c.AddTask<BlazorAdmin.Client.TestPages.Tasks.TestTask>("Task测试2", b => b.WithCron("*/12 * * * * ?").Build());
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
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies([.. AppConst.AdditionalAssemblies]);


app.Run();