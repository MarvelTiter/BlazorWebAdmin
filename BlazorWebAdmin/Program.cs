using BlazorWeb.Shared.Extensions;
using BlazorWebAdmin;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using LightExcel;
using MDbContext;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.SignalR;
using Project.AppCore.Auth;
using Project.AppCore.Store;
using Project.Common;
using System.Reflection;

WebApplicationOptions options = new WebApplicationOptions
{
	Args = args,
	ContentRootPath = AppContext.BaseDirectory
};
var builder = WebApplication.CreateBuilder(options);

var services = builder.Services;
// Add services to the container.
services.AddRazorPages();
services.AddServerSideBlazor();
services.AddControllers();
services.AddHttpClient();
services.SharedComponentsInit();
services.Configure<HubOptions>(options =>
{
	options.MaximumReceiveMessageSize = 1024 * 1024 * 2; // 1MB or use null
});

services.AddDataProtection().SetApplicationName("BlazorWebAdmin");

//services.AddAuthentication("Bearer")
//services.AddLocalization();
services.AddAntDesign();

services.AddLightExcel();
services.AutoInjects();
services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
services.AddHttpContextAccessor();

CustomSetup.SetupLightOrm(services, builder.Configuration);
CustomSetup.SetupCustomServices(services);

builder.Host.UseWindowsService();
var app = builder.Build();
CustomSetup.RegisterBlazorViewAssembly();
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Error");
}

CustomSetup.SetupCustomAppUsage(app);

app.UseStaticFiles();

app.UseRouting();
//app.UseRequestLocalization();
app.UseAuthentication();
app.UseAuthorization();

app.MapBlazorHub();
app.MapControllers();
app.MapFallbackToPage("/_Host");

app.Run();