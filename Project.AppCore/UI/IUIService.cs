using Microsoft.AspNetCore.Components;
using Project.Models;
using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Project.AppCore.UI
{
    public interface IInput<TItem, TValue>
    {
        IInput<TItem, TValue> Set<TMember>(Expression<Func<TItem, TMember>> selector, object value);
        IInput<TItem, TValue> Set(string key, object value);
        RenderFragment Render();
    }

    public interface IBindableInput<TItem, TValue> : IInput<TItem, TValue>
    {
        new IBindableInput<TItem, TValue> Set<TMember>(Expression<Func<TItem, TMember>> selector, object value);
        new IBindableInput<TItem, TValue> Set(string key, object value);
        IInput<TItem, TValue> Bind(Expression<Func<TValue>> expression);
    }


    public class ComponentBuilder<TComponent, TItem, TValue> : IInput<TItem, TValue> where TComponent : IComponent
    {
        protected readonly Dictionary<string, object?> parameters = new(StringComparer.Ordinal);

        public IInput<TItem, TValue> Set<TMember>(Expression<Func<TItem, TMember>> selector, object value)
        {
            var prop = GetProperty(selector);
            parameters.Add(prop.Name, value);
            return this;
        }

        public IInput<TItem, TValue> Set(string key, object value)
        {
            parameters.Add(key, value);
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

        protected static PropertyInfo GetProperty<T, V>(Expression<Func<T, V>> selector)
        {
            if (selector is null)
                throw new ArgumentNullException(nameof(selector));

            if (selector.Body is not MemberExpression expression || expression.Member is not PropertyInfo propInfoCandidate)
                throw new ArgumentException($"The parameter selector '{selector}' does not resolve to a public property on the type '{typeof(T)}'.", nameof(selector));
            var type = typeof(T);
            var propertyInfo = propInfoCandidate.DeclaringType != type
                             ? type.GetProperty(propInfoCandidate.Name, propInfoCandidate.PropertyType)
                             : propInfoCandidate;
            if (propertyInfo is null)
                throw new ArgumentException($"The parameter selector '{selector}' does not resolve to a public property on the type '{typeof(T)}'.", nameof(selector));

            return propertyInfo;
        }

    }

    public class BindableComponentBuilder<TComponent, TItem, TValue> : ComponentBuilder<TComponent, TItem, TValue>, IBindableInput<TItem, TValue> where TComponent : IComponent
    {
        public object Reciver { get; set; }

        protected TValue value;
        protected EventCallback<TValue> callback;
        protected Action<TValue> action;

        public new IBindableInput<TItem, TValue> Set<TMember>(Expression<Func<TItem, TMember>> selector, object value)
        {
            base.Set(selector, value);
            return this;
        }

        public new IBindableInput<TItem, TValue> Set(string key, object value)
        {
            base.Set(key, value);
            return this;
        }

        public IInput<TItem, TValue> Bind(Expression<Func<TValue>> expression)
        {
            var body = expression.Body;
            var p = Expression.Parameter(typeof(TValue), "v");
            var actionExp = Expression.Lambda<Action<TValue>>(Expression.Assign(body, p), p);
            action = actionExp.Compile();
            callback = EventCallback.Factory.Create(Reciver, action);
            var func = expression.Compile();
            value = func.Invoke();
            parameters.Add("Value", value);
            parameters.Add("ValueChanged", callback);
            return this;
        }
    }

    public class InputInfo<TValue>
    {
        public bool Disabled { get; set; }
        public TValue Value { get; set; }
        public EventCallback<TValue> ValueChanged { get; set; }
    }

    public class SelectInfo<TValue>
    {
        public object DataSource { get; set; }
        public TValue Value { get; set; }
        public EventCallback<TValue> ValueChanged { get; set; }
    }

    public interface IUIService
    {
        /// <summary>
        /// 生成输入框
        /// <code>
        /// UI.BuildInput(this).Bind(() => ValueExpression).Render()
        /// </code>
        /// </summary>
        IBindableInput<InputInfo<string>, string> BuildInput(object reciver);

        /// <summary>
        /// 生成密码输入框
        /// <code>
        /// UI.BuildInput(this).Bind(() => ValueExpression).Render()
        /// </code>
        /// </summary>
        IBindableInput<InputInfo<string>, string> BuildPassword(object reciver);
        /// <summary>
        /// 生成数字输入框
        /// <code>
        /// UI.BuildInput&lt;TValue&gt;(this).Bind(() => ValueExpression).Render()
        /// </code>
        /// </summary>
        IBindableInput<InputInfo<TValue>, TValue> BuildInput<TValue>(object reciver);

        /// <summary>
        /// 生成下拉选择框
        /// <code>
        /// UI.BuildSelect&lt;TValue&gt;(this, options).Bind(() => ValueExpression).Render()
        /// </code>
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="reciver"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        IBindableInput<SelectInfo<TValue>, TValue> BuildSelect<TValue>(object reciver, SelectItem<TValue> options);

    }
}
