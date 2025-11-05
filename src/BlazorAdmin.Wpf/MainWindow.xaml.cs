using Microsoft.AspNetCore.Components.WebView;
using Microsoft.Extensions.Hosting;
using System.ComponentModel;
using System.Windows;

namespace BlazorAdmin.Wpf;
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
        webview.Services = provider;
        webview.HostPage = "index.html";
        var root = new Microsoft.AspNetCore.Components.WebView.Wpf.RootComponent();
        root.Selector = "#app";
        root.ComponentType = typeof(WpfApp);
        webview.RootComponents.Add(root);
        webview.BlazorWebViewInitialized = BlazorWebViewInitialized;
    }

    private void BlazorWebViewInitialized(object? sender, BlazorWebViewInitializedEventArgs e)
    {

    }

    protected override void OnClosing(CancelEventArgs e)
    {
        var result = MessageBox.Show("确定退出系统?", "", MessageBoxButton.OKCancel);
        if (result == MessageBoxResult.Cancel)
        {
            e.Cancel = true;
        }
        base.OnClosing(e);
    }

    protected override void OnClosed(EventArgs e)
    {
        hostApplicationLifetime.StopApplication();
        base.OnClosed(e);
    }
}