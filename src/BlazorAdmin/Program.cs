using BlazorAdmin;
using Microsoft.AspNetCore.Authorization;
using Project.AppCore;
using Project.AppCore.Services;
using Project.Constraints;
using Project.UI.AntBlazor;
using Project.Web.Shared;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();
//.AddHubOptions(option =>
//{
//    option.MaximumReceiveMessageSize = 1024 * 1024 * 2;
//});

builder.Services.AddAntDesignUI();
builder.AddProjectService(setting =>
{
    setting.App.Name = "Demo";
    setting.App.Id = "Test";
    setting.App.Company = "Marvel";
    setting.ConfigureSettingProviderType<CustomSetting>();
    setting.ConfigureAuthService<DefaultAuthenticationService>();
});
builder.Services.AutoInject();
;
builder.AddDefaultLightOrm();

builder.Services.AddControllers();
var app = builder.Build();
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseWebAssemblyDebugging();
}

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseProject();
app.UseAntiforgery();
var rb = app.MapRazorComponents<App>();
rb.AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies([.. AppConst.Pages]);
app.MapControllers();

app.Run();
