using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Project.AppCore;
using Project.Services;
using Project.UI.AntBlazor;
namespace BlazorWinFormsAdmin
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();

            var builder = Host.CreateApplicationBuilder();

            builder.AddProject(setting =>
            {
                setting.App.Name = "Demo";
                setting.App.Id = "Test";
                setting.App.Company = "Marvel";
                setting.AddDefaultLogger = true;
                setting.ConfigureSettingProviderType<CustomSetting>();
            });
            builder.Services.AddAntDesignUI();
            builder.AddDefaultLightOrm();
            builder.Services.AddSingleton<MainForm>();
            builder.Services.AddHostedService<HostedWinformService<MainForm>>();
            builder.Services.AddWindowsFormsBlazorWebView();
            Console.WriteLine(Project.Constraints.AppConst.TempFilePath);
            Config.AddAssembly(typeof(WinformApp).Assembly);
            builder.Build().Run();
        }
    }
}