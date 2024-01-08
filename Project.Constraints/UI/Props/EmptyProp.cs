using Project.Common.Attributes;

namespace Project.Constraints.UI.Props
{
    [IgnoreAutoInject]
    public class DefaultProp
    {
        public string BindValueName { get; set; } = "Value";
    }
}
