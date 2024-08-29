using BlazorAdmin;
using Project.AppCore;
using Project.AppCore.Services;
using Project.Constraints;
using Project.UI.AntBlazor;
using Project.Web.Shared;
using _Imports = BlazorAdmin.Client._Imports;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddHubOptions(option =>
    {
        option.MaximumReceiveMessageSize = 1024 * 1024 * 2;
    })
    .AddInteractiveWebAssemblyComponents();

Extensions.AddAntDesignUI(builder.Services);
//Project.UI.FluentUI.Extensions.AddFluentUI(builder.Services);
//builder.Services.AddAntDesignUI();
//builder.Services.AddFluentUI();
builder.AddServerProject(setting =>
{
    setting.App.Name = "Demo";
    setting.App.Id = "Test";
    setting.App.Company = "Marvel";
    // 配置 IProjectSettingService和IAuthService
    // 默认分别为BasicSetting和DefaultAuthenticationService
    //setting.ConfigureSettingProviderType<CustomSetting>();
    //setting.ConfigureAuthService<DefaultAuthenticationService>();
#if DEBUG
    setting.ConfigureSettingProviderType<CustomSetting>();
    setting.ConfigureAuthService<DefaultAuthenticationService>();
#endif
});
builder.Services.AutoInject();

builder.AddDefaultLightOrm(options =>
{
    options.SetTableContext(new LightOrmTableContext());
});

builder.Services.AddControllers();

//AppConst.AddAssembly(typeof(_Imports));

var app = builder.Build();
// Configure the HTTP request pipeline.
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
    .AddAdditionalAssemblies([.. AppConst.Pages]);
app.MapControllers();

app.Run();