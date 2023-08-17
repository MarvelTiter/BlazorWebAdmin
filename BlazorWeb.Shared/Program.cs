using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.SignalR;
using Project.AppCore.Auth;
using Project.AppCore.Locales.Extensions;
using Project.AppCore.Options.Extensions;
using Project.AppCore.Options;
using BlazorWeb.Shared.Extensions;
using LightExcel;
using Project.AppCore;

namespace BlazorWeb.Shared
{
    public class Program
    {
        public static void Run(string appName
            , Action<WebApplicationBuilder>? builderOption
            , Action<WebApplication>? appOption
            , Func<IEnumerable<Type>>? registerAssembly
            , params string[] args)
        {
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
            services.AddControllers().AddApplicationPart(typeof(Project.AppCore.Program).Assembly);
            services.AddHttpClient();
            services.SharedComponentsInit();
            services.Configure<HubOptions>(options =>
            {
                options.MaximumReceiveMessageSize = 1024 * 1024 * 2; // 1MB or use null
            });

            services.AddDataProtection().SetApplicationName(appName);

            //services.AddAuthentication("Bearer")
            //services.AddLocalization();
            services.AddAntDesign();

            services.AddLightExcel();
            services.AutoInjects();
            services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
            services.AddHttpContextAccessor();
            services.AddJsonLocales();

            services.AddWebConfiguration<AppSetting>(app =>
            {
                builder.Configuration.GetSection(nameof(AppSetting)).Bind(app);
            });
            services.AddWebConfiguration<CultureOptions>(culture =>
            {
                builder.Configuration.GetSection(nameof(CultureOptions)).Bind(culture);
            });

            builderOption ??= CustomSetup.Setup;
            builderOption.Invoke(builder);

            registerAssembly ??= CustomSetup.RegisterBlazorViewAssembly;
            var types = registerAssembly.Invoke();
            Config.AddAssembly(types.ToArray());

            builder.Host.UseWindowsService();
            var app = builder.Build();
            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
            }
            appOption ??= CustomSetup.SetupCustomAppUsage;
            appOption.Invoke(app);

            ServiceLocator.Instance = app.Services;
            app.UseStaticFiles();

            app.UseRouting();
            //app.UseRequestLocalization();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapBlazorHub();
            app.MapControllers();
            app.MapFallbackToPage("/_Host");

            app.Run();
        }
    }
}
