using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Project.AppCore;
using Project.Services;
using Project.UI.AntBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorWpfAdmin
{
    public class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            var builder = Host.CreateApplicationBuilder(args);
            
            builder.AddProject(setting =>
            {
                setting.App.Name = "Demo";
                setting.App.Id = "Test";
                setting.App.Company = "Marvel";
                setting.AddFileLogger = true;
                setting.ConfigureSettingProviderType<CustomSetting>();
            });
            
            builder.AddDefaultLightOrm();
            builder.Services.AddSingleton<App>();
            builder.Services.AddTransient<MainWindow>();
            builder.Services.AddHostedService<WpfHostedService<App, MainWindow>>();
            builder.Services.AddWpfBlazorWebView();
            builder.Services.AddAntDesignUI();
            Config.AddAssembly(typeof(App).Assembly);
            Console.WriteLine(Project.Constraints.AppConst.TempFilePath);

            builder.Build().Run();
        }
    }
}
