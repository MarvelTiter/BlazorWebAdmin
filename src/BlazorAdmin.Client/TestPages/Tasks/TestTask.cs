using AutoInjectGenerator;
using MT.LightTask;

namespace BlazorAdmin.Client.TestPages.Tasks;

// [AutoInject(ServiceType = typeof(TestTask))]
[AutoInjectSelf]
public class TestTask : ITask
{
    public Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        Console.WriteLine($"Task测试2: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        return Task.CompletedTask;
    }
}