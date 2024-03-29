using Demo.Web.Components;
using Project.AppCore;
using Project.Services;
using Project.UI.AntBlazor;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddHubOptions(option =>
    {
        option.MaximumReceiveMessageSize = 1024 * 1024 * 5;
    });

builder.Services.AddAntDesignUI();
builder.AddProject(setting =>
{
    setting.App.Name = "Demo";
    setting.App.Id = "Test";
    setting.App.Company = "Marvel";
    setting.ConfigureSettingProviderType<CustomSetting>();
});

builder.AddDefaultLightOrm(option =>
{
    //option.InitializedContext<DbInitializer>();
});

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
    .AddAdditionalAssemblies([.. Config.Pages]);

app.Run();
