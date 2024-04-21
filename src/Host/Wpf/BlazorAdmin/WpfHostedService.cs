using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.Windows;

namespace BlazorWpfAdmin
{
    public class WpfHostedService<TApplication, TMainWindow> : IHostedService
         where TApplication : Application
         where TMainWindow : Window
    {
        public WpfHostedService(TApplication application, TMainWindow mainWindow, IHostApplicationLifetime hostApplicationLifetime, IConfiguration configuration)
        {
            this.application = application;
            this.mainWindow = mainWindow;
            this.configuration = configuration;
            hostApplicationLifetime.ApplicationStopping.Register(application.Shutdown);
        }

        private readonly TApplication application;
        private readonly TMainWindow mainWindow;
        private readonly IConfiguration configuration;

        public Task StartAsync(CancellationToken cancellationToken)
        {
            application.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            application.MainWindow.WindowState = WindowState.Maximized;
            application.MainWindow.Title = configuration.GetValue<string>("AppSetting:AppTitle");
            application.Run(mainWindow);
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
