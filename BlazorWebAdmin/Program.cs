using AspectCore.Extensions.DependencyInjection;
using Autofac.Extensions.DependencyInjection;
using BlazorWebAdmin;
using BlazorWebAdmin.Aop;
using BlazorWebAdmin.Auth;
using BlazorWebAdmin.Store;
using Microsoft.AspNetCore.Components.Authorization;
using Project.Common;
using Project.Common.Attributes;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;
// Add services to the container.
services.AddRazorPages();
services.AddServerSideBlazor();
services.AddECharts();

//
services.AddAntDesign();
//Ìæ»»Ä¬ÈÏµÄÈÝÆ÷
//builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
//services.ConfigureDynamicProxy(config =>
//{
//    config.Interceptors.Add(new LogAopFactory());
//    config.NonAspectPredicates.Add(m => m.CustomAttributes.All(a => a.AttributeType != typeof(LogInfoAttribute)));
//});

//services.AddScoped<LogAop>();
services.AutoInjects();
services.AddScoped<StoreTest>();
services.AddScoped<StateContainer>();
services.AddScoped<RouterStore>();
services.AddScoped<CounterStore>();
services.AddScoped<UserStore>();
services.AddScoped<EventDispatcher>();
services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
services.AddHttpContextAccessor();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}


app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
