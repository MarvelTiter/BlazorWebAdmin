using Microsoft.Extensions.Hosting;
using Project.Constraints;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Project.AppCore.BackgroundServices;

/// <summary>
/// 定义一个后台服务，用于定期清理临时文件夹中的文件。
/// </summary>
public class TempFileCleanUpService : BackgroundService
{
    /// <summary>
    /// 执行后台任务的主要方法。
    /// </summary>
    /// <param name="stoppingToken">用于停止任务执行的取消令牌。</param>
    /// <returns>一个任务对象。</returns>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // 获取临时文件夹的路径
        var folder = AppConst.TempFilePath;

        // 持续监控临时文件夹，直到服务被停止
        while (!stoppingToken.IsCancellationRequested)
        {
            // 如果临时文件夹不存在，则等待一小时后再继续检查
            if (!Directory.Exists(folder))
            {
                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
                continue;
            }

            // 获取临时文件夹中所有文件，并筛选出创建时间超过三天的文件
            var files = Directory.EnumerateFiles(folder).Where(file =>
            {
                var fi = new FileInfo(file);
                // 确保文件存在且创建时间超过三天
                return fi.Exists && (DateTime.Now - fi.CreationTime).Days >= 3;
            }).ToList();

            // 遍历并删除符合条件的文件
            files.ForEach(file => DeleteFile(file, stoppingToken));

            // 等待一天后再进行下一轮检查
            await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
        }
    }

    /// <summary>
    /// 删除指定文件。
    /// </summary>
    /// <param name="file">要删除的文件路径。</param>
    /// <param name="stoppingToken">用于停止任务执行的取消令牌。</param>
    private static void DeleteFile(string file, CancellationToken stoppingToken)
    {
        try
        {
            // 如果服务被停止，则不执行删除操作
            if (stoppingToken.IsCancellationRequested)
            {
                return;
            }

            // 删除文件
            File.Delete(file);
        }
        catch
        {
            // 忽略可能发生的任何异常
        }
    }
}