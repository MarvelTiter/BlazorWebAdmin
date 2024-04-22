using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorAdmin
{
    internal class HostedWinformService<TForm> : IHostedService
        where TForm : Form
    {
        private readonly TForm form;
        private readonly IConfiguration configuration;

        public HostedWinformService(TForm form, IHostApplicationLifetime hostApplication, IConfiguration configuration)
        {
            this.form = form;
            this.configuration = configuration;
            hostApplication.ApplicationStopping.Register(Application.Exit);
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            form.Text = configuration.GetValue<string>("AppSetting:AppTitle");
            Application.Run(form);
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
