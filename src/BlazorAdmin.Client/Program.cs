using AutoPageStateContainerGenerator;
using AutoWasmApiGenerator;
using BlazorAdmin.Client;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using BlazorTemplate.ClientCore;
using BlazorTemplate.ClientCore.Services;

//[assembly: GenerateApiInvoker]
var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.Services.ConfigureHttpClientDefaults(c =>
{
    //c.AddHttpMessageHandler<GeneratedApiHandler>();
    c.ConfigureHttpClient(h =>
    {
        h.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
    });
});
var useProxy = builder.Configuration.GetValue<bool>("AppSetting:UseAspectProxy");
if (useProxy)
{
    builder.ConfigureContainer(new AutoAopProxyGenerator.AutoAopProxyServiceProviderFactory());
}
//builder.Configuration
BlazorTemplate.UI.AntBlazor.Extensions.AddAntDesignUI(builder.Services);
//BlazorTemplate.UI.FluentUI.Extensions.AddFluentUI(builder.Services);
AppConst.AppAssembly = typeof(BlazorAdmin.Client._Imports).Assembly;
builder.Services.AddClientProject(builder.Configuration, setting =>
{
    setting.App.Id = "Test";
    setting.App.Name = "Demo";
    setting.App.Company = "Marvel";
#if DEBUG
    setting.ConfigureSettingProviderType<DevelopSetting>();
    setting.ConfigurePage(locator =>
    {
        locator.SetDashboardType<BlazorAdmin.Client.TestPages.TestDashboard>();
    });
#endif
}, out _);
builder.Services.AddStateContainers();
builder.Services.AutoInjectWasm();
#if DEBUG
// 会覆盖AddClientProject方法中默认的AddAuthorizationCore的配置
builder.Services.AddAuthorizationCore(o =>
{
    o.AddPolicy(AppConst.ONLINE_USER_POLICY, policy =>
    {
        policy.RequireUserName("admin");
    });
});
#endif

builder.Services.AddGeneratedApiClientServices();

var host = builder.Build();

await host.RunAsync();