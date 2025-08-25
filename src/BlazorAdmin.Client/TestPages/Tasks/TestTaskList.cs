using Microsoft.AspNetCore.Components;
using MT.LightTask;
using Project.Constraints.Common.Attributes;
using Project.Constraints.Models;
using Project.Constraints.Models.Request;
using Project.Constraints.UI.Table;
using Project.Web.Shared.Basic;
using System.Diagnostics.CodeAnalysis;

namespace BlazorAdmin.Client.TestPages.Tasks;

#if DEBUG
[Route("/testtasklist")]
[PageInfo(Title = "任务列表测试", Icon = "fa fa-question-circle-o", GroupId = "test")]
//[Layout(typeof(NotAuthorizedLayout))]
#endif
public class TestTaskList : ModelPage<TaskInfo, GenericRequest<TaskInfo>>
{

    [Inject, NotNull] ITaskCenter? Tc { get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        Options.ActionButtonColumn = 3;
        Options.ActionColumnWidth = "250";
    }
    protected override Task<QueryCollectionResult<TaskInfo>> OnQueryAsync(GenericRequest<TaskInfo> query)
    {
        var all = Tc.TaskSchedulers().Select(s => new TaskInfo
        {
            Name = s.Name,
            ScheduleStatus = s.ScheduleStatus,
            TaskStatus = s.TaskStatus,
            LastRuntime = s.Strategy?.LastRuntime,
            NextRuntime = s.Strategy?.NextRuntime,
            LastRunElapsedTime = s.Strategy?.LastRunElapsedTime
        });
        return Task.FromResult(all.CollectionResult());
    }

    [TableButton(Label = "开始")]
    public Task<IQueryResult?> Start(TaskInfo ti)
    {
        var tc = Tc.GetScheduler(ti.Name!)!;
        tc.Start();
        return QueryResult.Null().AsTask();
    }
    [TableButton(Label = "停止")]
    public Task<IQueryResult?> Stop(TaskInfo ti)
    {
        var tc = Tc.GetScheduler(ti.Name!)!;
        tc.Stop();
        return QueryResult.Null().AsTask();
    }
    [TableButton(Label = "立即执行")]
    public Task<IQueryResult?> RunImmediately(TaskInfo ti)
    {
        var tc = Tc.GetScheduler(ti.Name!)!;
        tc.RunImmediately();
        return QueryResult.Null().AsTask();
    }
}
