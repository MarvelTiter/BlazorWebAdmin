using Microsoft.AspNetCore.Components;
using System.Linq.Expressions;

namespace Project.Constraints.UI
{
    public class InputComponentBuilder<TComponent, TValue> : IBindableInput<TValue> where TComponent : IComponent
    {
        public object Reciver { get; set; }
        protected TValue value;
        protected EventCallback<TValue> callback;
        protected Action<TValue> assignAction;
        protected Func<TValue, Task> Callback;
        readonly Dictionary<string, object> parameters = new();
        public IBindableInput<TValue> AdditionalParameters(Dictionary<string, object> parameters)
        {
            if (parameters != null)
            {
                foreach (var item in parameters)
                {
                    this.parameters.Add(item.Key, item.Value);
                }
            }
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
            parameters.Add(valueName, value!);
            parameters.Add($"{valueName}Changed", callback);
            return this;
        }

        public IBindableInput<TValue> Set(string key, object value)
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
                    builder.AddMultipleAttributes(1, parameters);
                builder.CloseComponent();
            };
        }

    }

    public class InputComponentBuilder<TComponent, TPropModel, TValue> : InputComponentBuilder<TComponent, TValue>, IBindableInput<TPropModel, TValue> where TComponent : IComponent
    {
        public InputComponentBuilder()
        {

        }

        public void SetComponent<TProp>(Expression<Func<TComponent, TProp>> selector, object value)
        {

        }

        public new IBindableInput<TPropModel, TValue> Set(string key, object value)
        {
            base.Set(key, value);
            return this;
        }

        public IBindableInput<TPropModel, TValue> Set<TMember>(Expression<Func<TPropModel, TMember>> selector, object value)
        {
            throw new NotImplementedException();
        }
    }
}
