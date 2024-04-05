using Microsoft.AspNetCore.Components;

namespace Project.Constraints.UI.Flyout;

public class FlyoutOptions
{
    public string Title { get; set; }
    public string? Width { get; set; }
    public string? OkText { get; set; }
    public string? CancelText { get; set; }
    public Func<Task> OnOk { get; set; }
    public Func<Task> OnClose { get; set; }
    public RenderFragment Content { get; set; }
    public RenderFragment? Footer { get; set; }
    public bool ShowFooter { get; set; } = true;
}

public class PopoverOptions
{
    public string Title { get; set; }
    public RenderFragment Content { get; set; }
    public RenderFragment Trigger { get; set;}
    public Func<Task>? CloseAsync { get; set;}
}

public class FlyoutOptions<TReturn> : FlyoutOptions
{
    public IFeedback<TReturn>? Feedback { get; set; }
}

public enum Position
{
    Left, Top, Right, Bottom
}

public class FlyoutDrawerOptions<TReturn> : FlyoutOptions<TReturn>
{
    public Position Position { get; set; }
}

public struct FeedBackValue<TValue>
{
    public TValue? Value { get; set; }
    public bool Success { get; set; }
}

public interface IFeedback<TValue>
{
    Task<FeedBackValue<TValue>> OnOkAsync();
    Task OnCancelAsync();
}
