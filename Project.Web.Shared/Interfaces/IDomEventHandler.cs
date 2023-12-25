using Microsoft.AspNetCore.Components.Web;

namespace Project.Web.Shared.Interfaces
{
    public interface IDomEventHandler
    {
        event Func<MouseEventArgs, Task> BodyClickEvent;
        event Func<KeyboardEventArgs, Task> OnKeyDown;
        event Func<KeyboardEventArgs, Task> OnKeyUp;
    }

}
