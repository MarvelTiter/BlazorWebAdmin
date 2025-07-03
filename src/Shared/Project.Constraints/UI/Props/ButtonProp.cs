namespace Project.Constraints.UI.Props;

public class ButtonProp
{
    public ButtonType ButtonType { get; set; } = ButtonType.Default;
    public string? Text { get; set; }
    public bool FakeButton { get; set; }
}