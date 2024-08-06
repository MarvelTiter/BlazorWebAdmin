using BlazorAdmin.Client;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Project.Constraints;
using Project.Web.Shared;
using Project.UI.AntBlazor;
using Project.Constraints.Services;
using System.Text.Json;
using Project.Web.Shared.Services;
using LightExcel;
//[assembly: GenerateApiInvoker]
var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.Services.ConfigureHttpClientDefaults(c =>
{
    c.ConfigureHttpClient(h =>
    {
        h.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
    });
});
//builder.Configuration
builder.Services.AddAntDesignUI();
builder.Services.AddProject(builder.Configuration, setting =>
{
    setting.App.Name = "Demo";
    setting.App.Id = "Test";
    setting.App.Company = "Marvel";
    setting.ConfigureSettingProviderType<CustomSetting>();
}, out _);

builder.Services.AutoInjectWasm();
builder.Services.AddLightExcel();
builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IUserService, UserServiceApiInvoker>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationServiceApiInvoker>();
builder.Services.AddScoped<IPermissionService, PermissionServiceApiInvoker>();
builder.Services.AddScoped<IStandardPermissionService, StandardPermissionServiceApiInvoker>();
builder.Services.AddScoped<IStandardRunLogService, StandardRunLogServiceApiInvoker>();
builder.Services.AddScoped<IStandardUserService, StandardUserServiceApiInvoker>();
await builder.Build().RunAsync();

