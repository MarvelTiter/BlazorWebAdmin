using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Project.Constraints.UI;

/// <summary>
/// 支持点击事件的组件接口
/// </summary>
/// <typeparam name="TReturn"></typeparam>
public interface IClickable<out TReturn>
{
    TReturn OnClick(Action callback);
    TReturn OnClick(EventCallback callback);
    TReturn OnClick(Action<object> callback);
    TReturn OnClick(Func<Task> callback);
    TReturn OnClick(Func<object, Task> callback);
    TReturn OnClick(EventCallback<MouseEventArgs> callback);
    TReturn OnClick(Action<MouseEventArgs> callback);
    TReturn OnClick(Func<MouseEventArgs, Task> callback);
}
