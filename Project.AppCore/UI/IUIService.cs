using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Project.AppCore.UI.Table;
using Project.Models;
using Project.Models.Request;
using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Project.AppCore.UI
{
    public interface IUIComponent<TValue>
    {
        IUIComponent<TValue> Set(string key, object value);
        RenderFragment Render();
    }

    public interface IBindableInput<TValue>
    {
        IBindableInput<TValue> Set(string key, object value);
        IBindableInput<TValue> Bind(Expression<Func<TValue>> expression, string valueName = "Value");
    }

    public interface IButtonAction : IUIComponent<object>
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
    }

    public class ComponentBuilder<TComponent, TValue>: IUIComponent<TValue> where TComponent : IComponent
    {
        protected readonly Dictionary<string, object?> parameters = new(StringComparer.Ordinal);

        public IUIComponent<TValue> Set(string key, object value)
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

    public class BindableComponentBuilder<TComponent, TValue> : ComponentBuilder<TComponent, TValue>, IBindableInput<TValue> where TComponent : IComponent
    {
        public object Reciver { get; set; }

        protected TValue value;
        protected EventCallback<TValue> callback;
        protected Action<TValue> action;

        public new IBindableInput<TValue> Set(string key, object value)
        {
            base.Set(key, value);
            return this;
        }

        public IBindableInput<TValue> Bind(Expression<Func<TValue>> expression, string valueName = "Value")
        {
            var body = expression.Body;
            var p = Expression.Parameter(typeof(TValue), "v");
            var actionExp = Expression.Lambda<Action<TValue>>(Expression.Assign(body, p), p);
            action = actionExp.Compile();
            callback = EventCallback.Factory.Create(Reciver, action);
            var func = expression.Compile();
            value = func.Invoke();
            parameters.Add(valueName, value);
            parameters.Add($"{valueName}Changed", callback);
            return this;
        }
    }

    public class ButtonBuilder<TComponent> : BindableComponentBuilder<TComponent, object>, IButtonAction where TComponent : IComponent
    {
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

        public IButtonAction Text(string text)
        {
            Set("ChildContent", (RenderFragment)(builder => { builder.AddContent(0, text); }));
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
    public interface IUIService
    {
        void Message(MessageType type, string message);

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

        /// <summary>
        /// 生成下拉选择框
        /// <code>
        /// UI.BuildSelect&lt;TValue&gt;(this, options).Bind(() => ValueExpression).Render()
        /// </code>
        /// </summary>
        /// <returns></returns>
        IBindableInput<TValue> BuildSelect<TValue>(object reciver, SelectItem<TValue> options);

        /// <summary>
        /// 生成按钮
        /// </summary>
        IButtonAction BuildButton(object reciver);

        RenderFragment BuildTable<TModel, TQuery>(TableOptions<TModel, TQuery> options) where TQuery : IRequest, new();

    }
}
