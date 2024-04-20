using Microsoft.Extensions.DependencyInjection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.AspNetCore.Components.WebView;
using Project.UI.AntBlazor;
using Microsoft.Extensions.Hosting;

namespace BlazorWpfAdmin
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IHostApplicationLifetime hostApplicationLifetime;

        public MainWindow(IHostApplicationLifetime hostApplicationLifetime, IServiceProvider provider)
        {
            InitializeComponent();
            this.hostApplicationLifetime = hostApplicationLifetime;
            webview.HostPage = "index.html";
            webview.Services = provider;
            var root = new Microsoft.AspNetCore.Components.WebView.Wpf.RootComponent();
            root.Selector = "#app";
            root.ComponentType = typeof(WpfApp);
            webview.RootComponents.Add(root);
        }

        protected override void OnClosed(EventArgs e)
        {
            hostApplicationLifetime.StopApplication();
            base.OnClosed(e);
        }
    }
}