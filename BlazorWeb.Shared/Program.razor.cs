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
using Project.AppCore.Middlewares;
using AspectCore.Extensions.DependencyInjection;

namespace BlazorWeb.Shared
{
    public partial class Program
    {
        public static void Run<TApp>(string appName
           , Action<WebApplicationBuilder> builderOption
           , Action<WebApplication> appOption
           , Func<IEnumerable<Type>>? registerAssembly
           , params string[] args)
        {
#if RELEASE
            try
            {
                var processName = Process.GetCurrentProcess().ProcessName;
                bool isNewInstance;
                Mutex mtx = new Mutex(true, processName, out isNewInstance);
                if (!isNewInstance)
                {
                    var process = Process.GetProcessesByName(processName).FirstOrDefault();
                    process?.Kill();
                }
            }
            catch
            {
            }
#endif

            WebApplicationOptions options = new WebApplicationOptions
            {
                Args = args,
                ContentRootPath = AppContext.BaseDirectory
            };
            var builder = WebApplication.CreateBuilder(options);

            var services = builder.Services;
            // Add services to the container.

            services.AddRazorComponents().AddInteractiveServerComponents();

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

            services.AddWebConfiguration<Token>(token =>
            {
                builder.Configuration.GetSection(nameof(Token)).Bind(token);
            });



            //builderOption ??= DefaultSetup.Setup;
            builderOption.Invoke(builder);

            //registerAssembly ??= DefaultSetup.RegisterBlazorViewAssembly;
            var types = (registerAssembly?.Invoke() ?? Enumerable.Empty<Type>()).ToList();
            types.Add(typeof(Program));
            Config.AddAssembly(types.ToArray());

            builder.Host.UseWindowsService();
            services.ConfigureDynamicProxy();
            builder.Host.UseServiceProviderFactory(new DynamicProxyServiceProviderFactory());
            var app = builder.Build();
            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
            }
            //appOption ??= DefaultSetup.SetupCustomAppUsage;
            appOption.Invoke(app);

            ServiceLocator.Instance = app.Services;
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAntiforgery();
            //app.UseRequestLocalization();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapRazorComponents<TApp>().AddInteractiveServerRenderMode()
                .AddAdditionalAssemblies(typeof(Program).Assembly);
            //.AddAdditionalAssemblies(Config.Pages.ToArray());

            app.MapControllers();
            app.Run();
        }
    }
}
