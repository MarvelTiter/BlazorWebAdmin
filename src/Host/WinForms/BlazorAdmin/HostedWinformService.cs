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

        public HostedWinformService(TForm form, IHostApplicationLifetime hostApplication)
        {
            this.form = form;
            hostApplication.ApplicationStopping.Register(Application.Exit);
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            Application.Run(form);
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
