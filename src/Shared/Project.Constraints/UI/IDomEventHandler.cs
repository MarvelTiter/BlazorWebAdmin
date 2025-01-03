using Microsoft.AspNetCore.Components.Web;

namespace Project.Constraints.UI;

public interface IDomEventHandler
{
    event Func<MouseEventArgs, Task> BodyClickEvent;
    event Func<KeyboardEventArgs, Task> OnKeyDown;
    event Func<KeyboardEventArgs, Task> OnKeyUp;
    event Func<Task> OnThemeChanged;
}

public interface IThemeChangedBroadcast
{
    Task NotifyThemeChangedAsync();
}
