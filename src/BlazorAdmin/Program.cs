using BlazorAdmin;
using Microsoft.AspNetCore.Authentication;
using Project.AppCore;
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
    //setting.AddInterceotor<AdditionalTest>();
});
builder.Services.AutoInject();
;
//builder.Services.AddControllers().AddApplicationPart(typeof(Project.AppCore._Imports).Assembly);
builder.AddDefaultLightOrm();

builder.Services.AddControllers();
var app = builder.Build();
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseWebAssemblyDebugging();
}
app.UseAuthentication();
app.UseAuthorization();
app.UseProject();
app.UseAntiforgery();

var rb = app.MapRazorComponents<App>();
rb.AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies([.. AppConst.Pages]);
app.MapControllers();

app.Map("/logout", async (HttpContext context) =>
{
    await context.SignOutAsync();
    context.Response.Redirect("/");
});

app.Run();
