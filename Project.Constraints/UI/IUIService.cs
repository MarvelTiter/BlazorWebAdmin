using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
using Project.Common.Attributes;
using Project.Constraints.Store;
using Project.Constraints.UI.Dropdown;
using Project.Constraints.UI.Flyout;
using Project.Constraints.UI.Form;
using Project.Constraints.UI.Table;
using Project.Models;
using Project.Models.Forms;
using Project.Models.Request;
using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Project.Constraints.UI;
public interface IUIComponent
{
    IUIComponent Set(string key, object value);
    IUIComponent AdditionalParameters(Dictionary<string, object> parameters);
    RenderFragment Render();
}
public interface IUIComponent<TValue> : IUIComponent
{
    new IUIComponent<TValue> Set(string key, object value);
}

public interface IBindableInput<TValue> : IUIComponent<TValue>
{
    new IBindableInput<TValue> Set(string key, object value);
    IBindableInput<TValue> Bind(Expression<Func<TValue>> expression);
    IBindableInput<TValue> Bind(Expression<Func<TValue>> expression, Func<Task> onchange);
    IBindableInput<TValue> Bind(Expression<Func<TValue>> expression, string valueName, Func<Task>? onchange = null);
}

public interface ISelectInput<TItem, TValue> : IBindableInput<TValue>
{
    new ISelectInput<TItem, TValue> Set(string key, object value);
    ISelectInput<TItem, TValue> LabelExpression(Expression<Func<TItem, string>> expression);
    ISelectInput<TItem, TValue> ValueExpression(Expression<Func<TItem, TValue>> expression);
}

public interface IButtonAction : IUIComponent
{
    IButtonAction OnClick(Action callback);
    IButtonAction OnClick(EventCallback callback);
    IButtonAction OnClick(Action<object> callback);
    IButtonAction OnClick(Func<Task> callback);
    IButtonAction OnClick(Func<object, Task> callback);
    IButtonAction OnClick(EventCallback<MouseEventArgs> callback);
    IButtonAction OnClick(Action<MouseEventArgs> callback);
    IButtonAction OnClick(Func<MouseEventArgs, Task> callback);
    IButtonAction Text(string text);
    IButtonAction SetButtonType(ButtonType type);
}

public interface IColumnComponent
{
    IRowComponent AddContent(RenderFragment fragment);
}

public interface IRowComponent : IUIComponent<object>
{
    new IRowComponent Set(string key, object value);
    IRowComponent ChildContent(RenderFragment fragment);
    IColumnComponent AddCol(int span = 0);
}
[IgnoreAutoInject]
public class ComponentBuilder<TComponent> : IUIComponent where TComponent : IComponent
{
    protected readonly Dictionary<string, object?> parameters = new(StringComparer.Ordinal);
    public IUIComponent AdditionalParameters(Dictionary<string, object> parameters)
    {
        if (parameters != null)
        {
            foreach (var kv in parameters)
            {
                if (!this.parameters.ContainsKey(kv.Key))
                {
                    this.parameters[kv.Key] = kv.Value;
                }
            }
        }
        return this;
    }
    public IUIComponent Set(string key, object value)
    {
        parameters[key] = value;
        return this;
    }
    public virtual RenderFragment Render()
    {
        return builder =>
        {
            builder.OpenComponent<TComponent>(0);
            if (parameters.Count > 0)
                builder.AddMultipleAttributes(1, parameters!);
            builder.CloseComponent();
        };
    }
}

[IgnoreAutoInject]
public class ComponentBuilder<TComponent, TValue> : ComponentBuilder<TComponent>, IUIComponent<TValue> where TComponent : IComponent
{
    public new IUIComponent<TValue> Set(string key, object value)
    {
        base.Set(key, value);
        return this;
    }
}

[IgnoreAutoInject]
public class BindableComponentBuilder<TComponent, TValue> : ComponentBuilder<TComponent, TValue>, IBindableInput<TValue> where TComponent : IComponent
{
    public object Reciver { get; set; }

    protected TValue value;
    protected EventCallback<TValue> callback;
    protected Action<TValue> assignAction;
    protected Func<TValue, Task> Callback;

    public new IBindableInput<TValue> Set(string key, object value)
    {
        base.Set(key, value);
        return this;
    }
    public IBindableInput<TValue> Bind(Expression<Func<TValue>> expression)
    {
        return Bind(expression, "Value", null);
    }

    public IBindableInput<TValue> Bind(Expression<Func<TValue>> expression, Func<Task> onchange)
    {
        return Bind(expression, "Value", onchange);
    }

    public IBindableInput<TValue> Bind(Expression<Func<TValue>> expression, string valueName, Func<Task>? onchange = null)
    {
        /*
         * () => context.Value;
         * Action<TValue> : context.Value = v;
         *                  await onchange.Invoke();
         */
        var body = expression.Body;
        var p = Expression.Parameter(typeof(TValue), "v");
        var actionExp = Expression.Lambda<Action<TValue>>(Expression.Assign(body, p), p);
        assignAction = actionExp.Compile();
        Callback = v =>
        {
            assignAction.Invoke(v);
            if (onchange != null)
                return onchange.Invoke();
            return Task.CompletedTask;
        };

        callback = EventCallback.Factory.Create(Reciver, Callback);
        var func = expression.Compile();
        value = func.Invoke();
        parameters.Add(valueName, value);
        parameters.Add($"{valueName}Changed", callback);
        return this;
    }
}

[IgnoreAutoInject]
public class ButtonBuilder<TComponent> : BindableComponentBuilder<TComponent, object>, IButtonAction where TComponent : IComponent
{
    readonly Func<Dictionary<string, object?>, RenderFragment>? oveerrideRender;
    readonly Action<ButtonBuilder<TComponent>, Dictionary<string, object?>> attachParameters;
    public ButtonBuilder(Func<Dictionary<string, object?>, RenderFragment>? oveerrideRender)
    {
        this.oveerrideRender = oveerrideRender;
    }

    public ButtonBuilder(Action<ButtonBuilder<TComponent>, Dictionary<string, object?>> action)
    {
        attachParameters = action;
    }

    public ButtonBuilder()
    {

    }

    public override RenderFragment Render()
    {
        attachParameters?.Invoke(this, parameters);
        return oveerrideRender?.Invoke(parameters) ?? base.Render();
    }

    public IButtonAction OnClick(Action callback)
    {
        var onclick = EventCallback.Factory.Create<MouseEventArgs>(Reciver, callback);
        Set("OnClick", onclick);
        return this;
    }

    public IButtonAction OnClick(EventCallback callback)
    {
        var onclick = EventCallback.Factory.Create<MouseEventArgs>(Reciver, callback);
        Set("OnClick", onclick);
        return this;
    }

    public IButtonAction OnClick(Action<object> callback)
    {
        var onclick = EventCallback.Factory.Create<MouseEventArgs>(Reciver, callback);
        Set("OnClick", onclick);
        return this;
    }

    public IButtonAction OnClick(Func<Task> callback)
    {
        var onclick = EventCallback.Factory.Create<MouseEventArgs>(Reciver, callback);
        Set("OnClick", onclick);
        return this;
    }

    public IButtonAction OnClick(Func<object, Task> callback)
    {
        var onclick = EventCallback.Factory.Create<MouseEventArgs>(Reciver, callback);
        Set("OnClick", onclick);
        return this;
    }

    public IButtonAction OnClick(EventCallback<MouseEventArgs> callback)
    {
        var onclick = EventCallback.Factory.Create<MouseEventArgs>(Reciver, callback);
        Set("OnClick", onclick);
        return this;
    }

    public IButtonAction OnClick(Action<MouseEventArgs> callback)
    {
        var onclick = EventCallback.Factory.Create<MouseEventArgs>(Reciver, callback);
        Set("OnClick", onclick);
        return this;
    }

    public IButtonAction OnClick(Func<MouseEventArgs, Task> callback)
    {
        var onclick = EventCallback.Factory.Create<MouseEventArgs>(Reciver, callback);
        Set("OnClick", onclick);
        return this;
    }
    public ButtonType ButtonType { get; private set; } = ButtonType.Default;
    public IButtonAction SetButtonType(ButtonType type)
    {
        ButtonType = type;
        return this;
    }

    public IButtonAction Text(string text)
    {
        Set("ChildContent", (RenderFragment)(builder => { builder.AddContent(0, text); }));
        return this;
    }
}

[IgnoreAutoInject]
public class SelectBuilder<TComponent, TItem, TValue> : BindableComponentBuilder<TComponent, TValue>, ISelectInput<TItem, TValue> where TComponent : IComponent
{

    public ISelectInput<TItem, TValue> LabelExpression(Expression<Func<TItem, string>> expression)
    {
        var func = expression.Compile();
        parameters["LabelProperty"] = func;
        return this;
    }

    public ISelectInput<TItem, TValue> ValueExpression(Expression<Func<TItem, TValue>> expression)
    {
        var func = expression.Compile();
        parameters["ValueProperty"] = func;
        return this;
    }

    public new ISelectInput<TItem, TValue> Set(string key, object value)
    {
        base.Set(key, value);
        return this;
    }
}


public enum MessageType
{
    Success,
    Warning,
    Error,
    Information,
}

public enum ButtonType
{
    Default,
    Primary,
    Secondary,
    Danger,
    Success,
}
public interface IUIService
{
    void Message(MessageType type, string message);
    void Notify(MessageType type, string title, string message);
    Task<TReturn> ShowDialogAsync<TReturn>(FlyoutOptions<TReturn> options);
    Task<TReturn> ShowDrawerAsync<TReturn>(FlyoutDrawerOptions<TReturn> options);


    /// <summary>
    /// 生成输入框
    /// <code>
    /// UI.BuildInput(this).Bind(() => ValueExpression).Render()
    /// </code>
    /// </summary>
    IBindableInput<string> BuildInput(object reciver);

    /// <summary>
    /// 生成密码输入框
    /// <code>
    /// UI.BuildInput(this).Bind(() => ValueExpression).Render()
    /// </code>
    /// </summary>
    IBindableInput<string> BuildPassword(object reciver);
    /// <summary>
    /// 生成数字输入框
    /// <code>
    /// UI.BuildInput&lt;TValue&gt;(this).Bind(() => ValueExpression).Render()
    /// </code>
    /// </summary>
    IBindableInput<TValue> BuildInput<TValue>(object reciver);

    IBindableInput<TValue> BuildDatePicker<TValue>(object reciver);

    IBindableInput<bool> BuildCheckBox(object reciver);

    /// <summary>
    /// 生成下拉选择框
    /// <code>
    /// UI.BuildSelect&lt;TValue&gt;(this, options).Bind(() => ValueExpression).Render()
    /// </code>
    /// </summary>
    /// <returns></returns>
    IBindableInput<TValue> BuildSelect<TValue>(object reciver, SelectItem<TValue>? options);

    ISelectInput<TItem, TValue> BuildSelect<TItem, TValue>(object reciver, IEnumerable<TItem> options);

    /// <summary>
    /// 生成按钮
    /// </summary>
    IButtonAction BuildButton(object reciver);

    IBindableInput<bool> BuildSwitch(object reciver);

    RenderFragment BuildTable<TModel, TQuery>(TableOptions<TModel, TQuery> options) where TQuery : IRequest, new();

    RenderFragment BuildTableHeader<TModel, TQuery>(TableOptions<TModel, TQuery> options) where TQuery : IRequest, new();

    RenderFragment BuildForm<TData>(FormOptions<TData> options) where TData : class, new();

    RenderFragment BuildDropdown(DropdownOptions options);

    RenderFragment BuildMenu(IRouterStore router, bool horizontal, IAppStore app);

    RenderFragment BuildLoginForm(LoginFormModel model, Func<Task> handleLogin);

    IUIComponent BuildRow();
    IUIComponent BuildCard();
    IUIComponent BuildCol();
}
