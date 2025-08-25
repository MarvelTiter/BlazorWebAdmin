using Microsoft.AspNetCore.Components;
using MT.LightTask;
using Project.Constraints.Common.Attributes;
using Project.Constraints.UI;
using System.Diagnostics.CodeAnalysis;

namespace BlazorAdmin.Client.TestPages.Tasks;

#if DEBUG
[Route("/testcronexp")]
[PageInfo(Title = "CronExp测试", Icon = "fa fa-question-circle-o", GroupId = "test")]
//[Layout(typeof(NotAuthorizedLayout))]
#endif
public partial class TestCronExpression
{
    [Inject, NotNull] IUIService? UI { get; set; }
    [Inject, NotNull] ITaskCenter? Tc { get; set; }
    private string taskName = "测试";
    private string cronExpressionString = "0 5-10 19 ? * SAT";
    private CronExpression? exp;
    private List<(DateTimeOffset, DateTimeOffset)> values = [];

    private void Next()
    {
        exp = CronExpression.Parse(cronExpressionString);
        values.Clear();
        var now = DateTimeOffset.Now;
        var next = exp.GetNextOccurrence(now);
        values.Add((now, next));
    }

    private void NextTen()
    {
        exp = CronExpression.Parse(cronExpressionString);
        values.Clear();
        var now = DateTimeOffset.Now;
        for (int i = 0; i < 10; i++)
        {
            var next = exp.GetNextOccurrence(now);
            values.Add((now, next));
            now = next;
        }
    }

    private void CreateTask()
    {
        // "0 */10 * * * ?"
        Tc.AddTask<TestTask>(taskName, b => b.WithCron(cronExpressionString).Build());
    }
}