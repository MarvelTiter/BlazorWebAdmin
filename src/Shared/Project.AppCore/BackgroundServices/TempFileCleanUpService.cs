using Microsoft.Extensions.Hosting;
using Project.Constraints;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.AppCore.BackgroundServices
{
    public class TempFileCleanUpService : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var folder = AppConst.TempFilePath;
            while (!stoppingToken.IsCancellationRequested)
            {
                if (!Directory.Exists(folder))
                {
                    await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
                    continue;
                }
                var files = Directory.EnumerateFiles(folder).Where(file =>
                {
                    var fi = new FileInfo(file);
                    return fi.Exists && (DateTime.Now - fi.CreationTime).Days >= 3;

                }).ToList();
                files.ForEach(file => DeleteFile(file, stoppingToken));

                await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
            }
        }

        private static void DeleteFile(string file, CancellationToken stoppingToken)
        {
            try
            {
                if (stoppingToken.IsCancellationRequested)
                {
                    return;
                }
                File.Delete(file);
            }
            catch
            {
                //ignore
            }

        }
    }
}
