using Microsoft.AspNetCore.Components.WebView.WindowsForms;
using Microsoft.Extensions.Hosting;
using Microsoft.Web.WebView2.WinForms;
using System.ComponentModel;

namespace BlazorAdmin
{
    public partial class MainForm : Form
    {
        private readonly BlazorWebView blazorWebView;
        private readonly IHostApplicationLifetime hostApplication;

        public MainForm(IServiceProvider provider, IHostApplicationLifetime hostApplication)
        {
            InitializeComponent();
            blazorWebView = new BlazorWebView();
            blazorWebView.Dock = DockStyle.Fill;
            Controls.Add(blazorWebView);
            WindowState = FormWindowState.Maximized;
            blazorWebView.HostPage = "index.html";
            blazorWebView.Services = provider;
            blazorWebView.RootComponents.Add<WinformApp>("#app");
            this.hostApplication = hostApplication;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            var result = MessageBox.Show("确定退出系统?", "", MessageBoxButtons.OKCancel);
            if (result == DialogResult.Cancel)
            {
                e.Cancel = true;
            }
            base.OnClosing(e);
        }

        protected override void OnClosed(EventArgs e)
        {
            hostApplication.StopApplication();
            base.OnClosed(e);
        }
    }
}
