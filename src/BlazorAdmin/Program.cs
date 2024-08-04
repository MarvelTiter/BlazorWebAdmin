using BlazorAdmin;
using BlazorAdmin.TestPages;
using Project.AppCore;
using Project.Constraints;
using Project.Constraints.Models.Permissions;
using Project.Constraints.Models.Request;
using Project.Constraints.Services;
using Project.Services;
using Project.UI.AntBlazor;
using Project.Web.Shared.Pages;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddHubOptions(option =>
    {
        option.MaximumReceiveMessageSize = 1024 * 1024 * 2;
    });

builder.Services.AddAntDesignUI();
builder.AddProject(setting =>
{
    setting.App.Name = "Demo";
    setting.App.Id = "Test";
    setting.App.Company = "Marvel";
    setting.ConfigureSettingProviderType<CustomSetting>();
    setting.AddInterceotor<AdditionalTest>();
});

builder.Services.AutoInject();

//builder.Services.AddControllers().AddApplicationPart(typeof(Project.AppCore._Imports).Assembly);
builder.AddDefaultLightOrm();

var app = builder.Build();
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}
app.UseProject();
app.UseAntiforgery();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddAdditionalAssemblies([.. AppConst.Pages]);
//app.MapControllers();
app.Run();
