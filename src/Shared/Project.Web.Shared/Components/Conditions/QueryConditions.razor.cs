using Microsoft.AspNetCore.Components;
using System.Linq.Expressions;
using Project.Constraints.Page;
using Project.Constraints.UI;
using Project.Web.Shared.Utils;

namespace Project.Web.Shared.Components
{
    public interface IQueryCondition
    {
        int Column { get; set; }
        int LabelWidth { get; set; }
        int IndexFixed { get; set; }
        void AddCondition(ICondition condition);
        IList<ICondition> Conditions { get; set; }
        Task UpdateCondition(int index, ConditionInfo info);
    }
    public partial class QueryConditions<TItem> : BasicComponent, IQueryCondition
    {
        [Parameter]
        public int Column { get; set; } = 5;
        [Parameter]
        public int? ColumnMinWidth { get; set; }
        [Parameter]
        public int LabelWidth { get; set; } = 100;
        [Parameter]
        public string Gap { get; set; } = "5px";
        [Parameter, NotNull]
        public RenderFragment? ChildContent { get; set; }
        [Parameter]
        public Expression<Func<TItem, bool>>? Expression { get; set; }
        [Parameter]
        public EventCallback<Expression<Func<TItem, bool>>> ExpressionChanged { get; set; }

        public IList<ICondition> Conditions { get; set; } = new List<ICondition>();
        
        public int IndexFixed { get; set; }


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
