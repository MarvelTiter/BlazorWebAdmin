using BlazorAdmin.Client;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Project.Constraints;
using Project.Constraints.Services;
using Project.Web.Shared;

//[assembly: GenerateApiInvoker]
var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.Services.ConfigureHttpClientDefaults(c =>
{
    c.ConfigureHttpClient(h => { h.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress); });
});
//builder.Configuration
Project.UI.AntBlazor.Extensions.AddAntDesignUI(builder.Services);
//Project.UI.FluentUI.Extensions.AddFluentUI(builder.Services);
builder.Services.AddProject(builder.Configuration, setting =>
{
    setting.App.Name = "Demo";
    setting.App.Id = "Test";
    setting.App.Company = "Marvel";
    setting.ConfigureSettingProviderType<CustomSetting>();
}, out _);
builder.Services.AutoInjectWasm();
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<IUserService, UserServiceApiInvoker>();
builder.Services.AddScoped<IAuthService, AuthServiceApiInvoker>();
builder.Services.AddScoped<IPermissionService, PermissionServiceApiInvoker>();
builder.Services.AddScoped<IStandardPermissionService, StandardPermissionServiceApiInvoker>();
builder.Services.AddScoped<IStandardRunLogService, StandardRunLogServiceApiInvoker>();
builder.Services.AddScoped<IStandardUserService, StandardUserServiceApiInvoker>();

await builder.Build().RunAsync();