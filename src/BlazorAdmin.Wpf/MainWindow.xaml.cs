using Microsoft.AspNetCore.Components.WebView;
using Microsoft.Extensions.DependencyInjection;
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
        var root = new Microsoft.AspNetCore.Components.WebView.Wpf.RootComponent
        {
            Selector = "#app",
            ComponentType = typeof(Routers)
        };
        var head = new Microsoft.AspNetCore.Components.WebView.Wpf.RootComponent
        {
            Selector = "#head",
            ComponentType = typeof(WpfHead)
        };
        webview.RootComponents.Add(head);
        webview.RootComponents.Add(root);
        webview.BlazorWebViewInitialized = BlazorWebViewInitialized;
        //webview.WebView.CoreWebView2
    }

    private void BlazorWebViewInitialized(object? sender, BlazorWebViewInitializedEventArgs e)
    {
        //e.WebView.CoreWebView2.
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