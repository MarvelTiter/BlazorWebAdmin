using Project.Common.Attributes;
using System.Linq.Expressions;

namespace Project.Constraints.UI.Props
{
    [IgnoreAutoInject]
    public class SelectProp : DefaultProp
    {
        public bool AllowClear { get; set; } = true;
        public Expression LabelExpression { get; set; }
        public Expression ValueExpression { get; set; }
    }
}
