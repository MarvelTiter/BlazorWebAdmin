using BlazorWebAdmin.Store;
using Project.Common;
using Project.IRepositories;
using Project.IServices;
using Project.Repositories;
using Project.Services;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;
// Add services to the container.
services.AddRazorPages();
services.AddServerSideBlazor();
services.AddECharts();

services.AddScoped<RouterStore>();
services.AddScoped<CounterStore>();
services.AddScoped<UserStore>();
services.AddScoped<EventDispatcher>();

services.AddScoped<IAllAccess,AllAccess>();
services.AddScoped<IUserService, UserService>();
services.AddScoped<IUserRepository, UserRepository>();
//
services.AddAntDesign();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}


app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
