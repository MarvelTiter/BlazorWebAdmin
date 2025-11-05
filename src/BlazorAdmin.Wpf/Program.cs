using AutoPageStateContainerGenerator;
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
    [STAThread]
    public static void Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);

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
        builder.Services.AutoInject();
        builder.Services.AutoInjectHybrid();
        builder.Services.AddStateContainers();
        builder.Services.AddSingleton<App>();
        builder.Services.AddTransient<MainWindow>();
        builder.Services.AddHostedService<WpfHostedService<App, MainWindow>>();
        builder.Services.AddWpfBlazorWebView();
        //builder.Services.AddAuthenticationCore();
        //builder.Services.AddAuthorization();
        Console.WriteLine(AppConst.TempFilePath);
        var host = builder.Build();
        host.Run();
    }
}
