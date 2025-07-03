using Microsoft.AspNetCore.Components;
using System.Linq.Expressions;
using Project.Constraints.Page;
using Project.Constraints.UI;
using Project.Web.Shared.Utils;
using Project.Constraints.Models.Request;

namespace Project.Web.Shared.Components;

public interface IQueryCondition
{
    int Column { get; set; }
    int LabelWidth { get; set; }
    int? ContentWidth { get; set; }
    int? ColumnWidth { get; set; }
    int IndexFixed { get; set; }
    void AddCondition(ICondition condition);
    IList<ICondition> Conditions { get; set; }
    void UpdateCondition(int index, ConditionUnit info);
    bool TryGetCondition(int index, out ConditionUnit? unit);
}
public partial class QueryConditions : AppComponentBase, IQueryCondition
{
    [Parameter] public int Column { get; set; } = 5;
    [Parameter] public int? ColumnWidth { get; set; }
    [Parameter] public int LabelWidth { get; set; } = 100;
    [Parameter] public int? ContentWidth { get; set; }
    [Parameter] public int Gap { get; set; } = 8;
    [Parameter] public RenderFragment? Header { get; set; }
    [Parameter] public RenderFragment? HeaderButtons { get; set; }
    [Parameter, NotNull] public RenderFragment? ChildContent { get; set; }
    [Parameter] public IRequest? Request { get; set; }
    [Parameter] public bool StackLayout { get; set; }
    //[Parameter] public EventCallback<ConditionUnit> ConditionChanged { get; set; }
    private bool expand;
    protected override void OnInitialized()
    {
        base.OnInitialized();
        if (Request != null)
        {
            Request.ExpressionSolveType = SolveType.All;
        }
    }
    public IList<ICondition> Conditions { get; set; } = new List<ICondition>();

    public int IndexFixed { get; set; }


    public void AddCondition(ICondition condition)
    {
        if (condition == null) return;
        Conditions.Add(condition);
    }

    private readonly Dictionary<int, ConditionUnit> infos = [];
    public void UpdateCondition(int index, ConditionUnit info)
    {
        infos[index] = info;
        //var exp = BuildCondition.CombineExpression<TItem>(new Queue<ConditionInfo>(infos.Values));
        //Console.WriteLine(exp);
        if (Request != null)
        {
            Request.Condition.Children = [.. infos.Values];
        }
    }
    public bool TryGetCondition(int index, out ConditionUnit? unit) => infos.TryGetValue(index, out unit);
}