using MT.LightTask;
using Project.Constraints.Common.Attributes;

namespace BlazorAdmin.Client.TestPages.Tasks;

public class TaskInfo
{
    [ColumnDefinition("任务名称")]
    public string? Name { get; set; }
    [ColumnDefinition("调度状态")]
    public TaskScheduleStatus ScheduleStatus { get; internal set; }
    [ColumnDefinition("任务状态")]
    public TaskRunStatus TaskStatus { get; internal set; }
    [ColumnDefinition("最后运行时间", Format = "yyyy-MM-dd HH:mm:ss")]
    public DateTimeOffset? LastRuntime { get; internal set; }
    [ColumnDefinition("下次运行时间", Format = "yyyy-MM-dd HH:mm:ss")]
    public DateTimeOffset? NextRuntime { get; internal set; }
    [ColumnDefinition("最后运行耗时", Format = "dd\\-hh\\:mm\\:ss")]
    public TimeSpan? LastRunElapsedTime { get; internal set; }
}
