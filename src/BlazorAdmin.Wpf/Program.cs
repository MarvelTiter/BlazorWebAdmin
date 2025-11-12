using AutoPageStateContainerGenerator;
using BlazorAdmin.Wpf.Auth;
using LightORM;
using LightORM.Providers.Sqlite.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Project.AppCore;
using Project.AppCore.Services;
using Project.Constraints;
using Project.Constraints.Services;
using Project.Web.Shared;
namespace BlazorAdmin.Wpf;

public class Program
{
    public static class BlazorHybridWpfApplication
    {
        public static HostApplicationBuilder Create(string[] args)
        {
            var configuration = new ConfigurationManager();
            configuration.AddEnvironmentVariables("ASPNETCORE_");
            var builder = Host.CreateApplicationBuilder(new HostApplicationBuilderSettings()
            {
                Args = args,
                Configuration = configuration
            });
            return builder;
        }
    }
    [STAThread]
    public static void Main(string[] args)
    {
        var builder = BlazorHybridWpfApplication.Create(args);
        Project.UI.AntBlazor.Extensions.AddAntDesignUI(builder.Services);
        //Project.UI.FluentUI.Extensions.AddFluentUI(builder.Services);
        //builder.Services.AddAntDesignUI();
        //builder.Services.AddFluentUI();
        builder.Services.AddClientProject(builder.Configuration, setting =>
        {
            setting.App.Id = "Test";
            setting.App.Name = "Demo";
            setting.App.Company = "Marvel";

            var appAssembly = typeof(Program).Assembly;

            AppConst.AppAssembly = appAssembly;
            /*
             * 配置IProjectSettingService和IAuthService(如果需要)
             */
#if DEBUG
            setting.ConfigureSettingProviderType<CustomSetting>();
#endif
#if (ExcludeDefaultService)
    // setting.ConfigureSettingProviderType<YourSetting>();
    // setting.ConfigureSettingProviderType<BasicSetting>();
    // setting.ConfigureAuthService<YourAuthenticationService>();
#endif
        }, out var setting);
        var connStr = builder.Configuration.GetConnectionString("Sqlite")!;
        builder.Services.AddLightOrm(option =>
        {
            option.UseSqlite(connStr);
            option.SetTableContext<LightOrmTableContext>();
            option.UseInterceptor<LightOrmSqlTrace>();
        });
        builder.Services.AutoInjectWpf();

        builder.Services.AddStateContainers();
        builder.Services.AddSingleton<App>();
#if (!ExcludeDefaultService)
        builder.Services.AddScoped<IAuthService, LocalAuthService>();
#endif
        builder.Services.AddSingleton<MainWindow>();
        builder.Services.AddHostedService<WpfHostedService<App, MainWindow>>();
        builder.Services.AddWpfBlazorWebView();
        if (builder.Environment.IsDevelopment())
        {
            builder.Services.AddBlazorWebViewDeveloperTools();
        }
        var host = builder.Build();

        host.Run();
    }
}
