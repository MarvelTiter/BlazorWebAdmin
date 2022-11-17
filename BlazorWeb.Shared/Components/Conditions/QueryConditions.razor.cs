using BlazorWeb.Shared.Template.Tables.Setting;
using Microsoft.AspNetCore.Components;
using System.Linq.Expressions;

namespace BlazorWeb.Shared.Components
{
    public interface IQueryCondition
    {
        int ColWidth { get; set; }
        int LabelWidth { get; set; }
        void AddCondition(ICondition condition);
        IList<ICondition> Conditions { get; set; }
        Task UpdateCondition(int index, ConditionInfo info);
    }
    public partial class QueryConditions<TItem> : ComponentBase, IQueryCondition
    {
        [Parameter]
        public int ColWidth { get; set; } = 6;
        [Parameter]
        public int LabelWidth { get; set; } = 100;
        [Parameter]
        public RenderFragment ChildContent { get; set; }
        [Parameter]
        public Expression<Func<TItem, bool>> Expression { get; set; }
        [Parameter]
        public EventCallback<Expression<Func<TItem, bool>>> ExpressionChanged { get; set; }
        public IList<ICondition> Conditions { get; set; } = new List<ICondition>();
        public void AddCondition(ICondition condition)
        {
            if (condition == null) return;
            Conditions.Add(condition);
        }
        Dictionary<int, ConditionInfo> infos = new Dictionary<int, ConditionInfo>();
        public async Task UpdateCondition(int index, ConditionInfo info)
        {
            infos[index] = info;
            var exp = BuildCondition.CombineExpression<TItem>(new Queue<ConditionInfo>(infos.Values));
            //Console.WriteLine(exp);
            await ExpressionChanged.InvokeAsync(exp);
        }
    }
}
