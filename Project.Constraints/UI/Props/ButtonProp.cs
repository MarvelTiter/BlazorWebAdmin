namespace Project.Constraints.UI.Props
{
    [IgnoreAutoInject]
    public class ButtonProp
    {
        public ButtonType ButtonType { get; set; } = ButtonType.Default;
        public string? Text { get; set; }
    }
}
