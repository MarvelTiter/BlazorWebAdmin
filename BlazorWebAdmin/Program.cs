using BlazorWebAdmin;
using MDbContext;
using Microsoft.AspNetCore.Components.Authorization;
using Project.AppCore.Auth;
using Project.AppCore.Store;
using Project.Common;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;
// Add services to the container.
services.AddRazorPages();
services.AddServerSideBlazor();

//
services.AddAntDesign();
services.UseLightOrm(config =>
{
    config.SetDatabase(DbBaseType.Sqlite, Project.AppCore.LightDb.CreateConnection)
    .SetWatcher(option =>
    {
        option.BeforeExecute = sql =>
        {
            Console.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} Sql => \n{sql}\n");
        };
    });
});
services.AddSessionStorageServices();
services.AutoInjects();
//services.AddScoped<StateContainer>();
//services.AddScoped<RouterStore>();
//services.AddScoped<CounterStore>();
//services.AddScoped<UserStore>();
//services.AddScoped<EventDispatcher>();
services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
services.AddHttpContextAccessor();
var app = builder.Build();
Config.AddAssembly(typeof(BlazorWeb.UI.Program));
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

//TODO 界面组件独立库
//TODO Camera组件
//