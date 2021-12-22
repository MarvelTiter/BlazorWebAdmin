using BlazorWebAdmin.Common;
using BlazorWebAdmin.IRepositories;
using BlazorWebAdmin.IServices;
using BlazorWebAdmin.Repositories;
using BlazorWebAdmin.Services;
using BlazorWebAdmin.Store;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;
// Add services to the container.
services.AddRazorPages();
services.AddServerSideBlazor();

services.AddScoped<RouterStore>();
services.AddScoped<CounterStore>();
services.AddScoped<UserStore>();
services.AddScoped<EventDispatcher>();

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
