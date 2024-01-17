namespace Project.Constraints.UI.Props
{
    [IgnoreAutoInject]
    public class DefaultProp
    {
        public bool EnableValueExpression { get; set; } = true;
        public string BindValueName { get; set; } = "Value";
    }
}
