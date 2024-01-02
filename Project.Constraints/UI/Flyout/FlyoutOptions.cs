using Microsoft.AspNetCore.Components;

namespace Project.Constraints.UI.Flyout;

public class FlyoutOptions
{
    public string Title { get; set; }
    public object? Width { get; set; }
    public Func<Task> OnOk { get; set; }
    public Func<Task> OnClose { get; set; }
    public RenderFragment Content { get; set; }
    public RenderFragment Footer { get; set; }
}

public class FlyoutOptions<TReturn> : FlyoutOptions
{
    public IFeedback<TReturn>? Feedback { get; set; }
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
