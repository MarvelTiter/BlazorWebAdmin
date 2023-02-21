using BlazorWebAdmin;
using LightExcel;
using MDbContext;
using Microsoft.AspNetCore.Components.Authorization;
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
//services.AddAuthentication("Bearer")
    
//
services.AddAntDesign();
services.AddLightOrm(option =>
{
    option.SetDatabase(DbBaseType.Sqlite, Project.AppCore.LightDb.CreateConnection)
    .SetWatcher(sql =>
    {
        sql.BeforeExecute = e =>
        {
#if DEBUG
            Console.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} Sql => \n{e.Sql}\n");
#endif
        };
    });
});
services.AddLightExcel();
//services.AddSessionStorageServices();
services.AutoInjects();
//services.AddScoped<StateContainer>();
//services.AddScoped<RouterStore>();
//services.AddScoped<CounterStore>();
//services.AddScoped<UserStore>();
//services.AddScoped<EventDispatcher>();
services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
services.AddHttpContextAccessor();
builder.Host.UseWindowsService();
var app = builder.Build();
Config.AddAssembly(typeof(BlazorWeb.Shared.Program));
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
app.MapControllers();
app.MapFallbackToPage("/_Host");

app.Run();

//TODO 界面组件独立库
//TODO Camera组件
//