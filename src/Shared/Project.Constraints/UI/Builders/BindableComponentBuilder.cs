using Microsoft.AspNetCore.Components;
using Project.Constraints.Common;
using Project.Constraints.UI.Props;
using System.Linq.Expressions;

namespace Project.Constraints.UI.Builders
{
    public class BindableComponentBuilder<TComponent, TPropModel, TValue> : PropComponentBuilder<TComponent, TPropModel>, IBindableInputComponent<TPropModel, TValue>
        where TComponent : IComponent
        where TPropModel : DefaultProp, new()
    {

        protected TValue? value;
        protected EventCallback<TValue>? callback;
        protected Action<TValue>? assignAction;
        protected Func<TValue, Task>? Callback;
        public BindableComponentBuilder()
        {

        }

        public BindableComponentBuilder(Func<PropComponentBuilder<TComponent, TPropModel>, RenderFragment> newRender)
        {
            this.newRender = newRender;
        }

        public BindableComponentBuilder(Action<PropComponentBuilder<TComponent, TPropModel>> tpropHandle)
        {
            this.tpropHandle = tpropHandle;
        }

        public IBindableInputComponent<TPropModel, TValue> Bind(Expression<Func<TValue>> expression)
        {
            return Bind(expression, null);
        }

        public IBindableInputComponent<TPropModel, TValue> Bind(Expression<Func<TValue>> expression, Func<Task>? onchange)
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

            callback = EventCallback.Factory.Create(Receiver, Callback);
            var func = expression.Compile();
            value = func.Invoke();
            parameters.Add(Model.BindValueName, value!);
            parameters.Add($"{Model.BindValueName}Changed", callback);
            if (Model.EnableValueExpression)
            {
                parameters.Add("ValueExpression", expression);
            }
            return this;
        }
    }
}
