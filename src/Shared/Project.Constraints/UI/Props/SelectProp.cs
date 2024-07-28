using System.Linq.Expressions;

namespace Project.Constraints.UI.Props
{
    public class SelectProp : DefaultProp
    {
        public bool AllowClear { get; set; } = true;
        public bool AllowSearch { get; set; } = true;
        public Expression? LabelExpression { get; set; }
        public Expression? ValueExpression { get; set; }
    }
}
