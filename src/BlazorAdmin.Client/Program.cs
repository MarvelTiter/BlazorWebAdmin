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
builder.Services.AddClientProject(builder.Configuration, setting =>
{
    setting.App.Id = "Test";
    setting.App.Name = "Demo";
    setting.App.Company = "Marvel";
#if DEBUG
    setting.ConfigureSettingProviderType<CustomSetting>();
#endif
}, out _);
builder.Services.AutoInjectWasm();
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<IAuthService, AuthServiceApiInvoker>();
builder.Services.AddScoped<IClientService, ClientServiceApiInvoker>();
builder.Services.AddScoped<ISvgIconService, SvgIconServiceApiInvoker>();
builder.Services.AddScoped<IFileService, FileServiceApiInvoker>();
#if (ExcludeDefaultService)
#else
// builder.Services.AddScoped<IUserService, UserServiceApiInvoker>();
builder.Services.AddScoped<IPermissionService, PermissionServiceApiInvoker>();
builder.Services.AddScoped<IStandardPermissionService, StandardPermissionServiceApiInvoker>();
builder.Services.AddScoped<IStandardRunLogService, StandardRunLogServiceApiInvoker>();
builder.Services.AddScoped<IStandardUserService, StandardUserServiceApiInvoker>();
#endif
await builder.Build().RunAsync();