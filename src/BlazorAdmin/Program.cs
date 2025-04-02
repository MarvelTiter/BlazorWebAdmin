using BlazorAdmin;
using LightORM;
using LightORM.Providers.Sqlite.Extensions;
using MT.Toolkit.LogTool;
using Project.AppCore;
using Project.AppCore.Services;
using Project.Constraints;
using Project.Web.Shared;
using MT.LightTask;
using BlazorAdmin.Client.TestPages.Tasks;
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
    // 配置 IProjectSettingService和IAuthService
    // 默认分别为BasicSetting和DefaultAuthenticationService
    //setting.ConfigureSettingProviderType<CustomSetting>();
    //setting.ConfigureAuthService<DefaultAuthenticationService>();
    var appAssembly  = typeof(BlazorAdmin.Client._Imports).Assembly;
    AppConst.AppAssembly = appAssembly;
    AppConst.AddAssembly(appAssembly);
#if DEBUG
    setting.ConfigureSettingProviderType<CustomSetting>();
#endif
    setting.ConfigureAuthService<DefaultAuthenticationService>();
});
//#if (ExcludeDefaultService)
//#else

//#endif
builder.Logging.AddLocalFileLogger(config =>
{
    config.LogFileSize = 1024 * 1024 * 5;
});

var connStr = builder.Configuration.GetConnectionString("Sqlite")!;
builder.Services.AddLightOrm(option =>
{
    option.UseSqlite(connStr);
    option.SetTableContext(new LightOrmTableContext());
    option.SetWatcher(sql =>
    {
        sql.DbLog = (s, p) =>
        {
            Console.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} Sql => \n{s}\n");
        };
    });
});

builder.Services.AutoInject();

builder.Services.AddControllers();

builder.Services.AddLightTask();

var app = builder.Build();
// Configure the HTTP request pipeline.
app.UseLightTask(c =>
{
    c.AddTask("测试1", (sp, token) =>
    {
        Console.WriteLine($"Task测试1: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        return Task.CompletedTask;
    }, b => b.WithCron("*/12 * * * * ?"));
});

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

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies([.. AppConst.AdditionalAssemblies]);
app.MapControllers();

app.Run();