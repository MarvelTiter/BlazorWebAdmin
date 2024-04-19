using Microsoft.AspNetCore.Components;

namespace Project.Constraints.UI.Props
{
    [IgnoreAutoInject]
    public class DefaultProp
    {
        public string? Label { get; set; }
        public bool EnableValueExpression { get; set; } = true;
        public string BindValueName { get; set; } = "Value";
    }
}
