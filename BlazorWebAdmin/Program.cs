using BlazorWebAdmin;
using Microsoft.AspNetCore.Components.Authorization;
using Project.ApplicationStore;
using Project.ApplicationStore.Auth;
using Project.ApplicationStore.Store;
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

services.AddSessionStorageServices();
services.AutoInjects();
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
