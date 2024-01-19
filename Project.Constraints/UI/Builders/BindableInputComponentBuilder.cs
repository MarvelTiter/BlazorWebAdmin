﻿using Microsoft.AspNetCore.Components;
using Project.Constraints.Common;
using Project.Constraints.UI.Props;
using System.Linq.Expressions;

namespace Project.Constraints.UI.Builders
{
    [IgnoreAutoInject]
    public class BindableInputComponentBuilder<TComponent, TPropModel, TValue, TSelf> : ComponentBuilder<TComponent, TSelf>, IBindableInputComponent<TPropModel, TValue>
        where TComponent : IComponent
        where TPropModel : DefaultProp, new()
        where TSelf : ComponentBuilder<TComponent, TSelf>
    {

        public TPropModel Model { get; set; } = new TPropModel();

        protected TValue value;
        protected EventCallback<TValue> callback;
        protected Action<TValue> assignAction;
        protected Func<TValue, Task> Callback;

        public BindableInputComponentBuilder()
        {

        }

        public BindableInputComponentBuilder(Func<TSelf, RenderFragment> newRender)
        {
            this.newRender = newRender;
        }

        public BindableInputComponentBuilder(Action<TSelf> tpropHandle)
        {
            this.tpropHandle = tpropHandle;
        }

        public IBindableInputComponent<TPropModel, TValue> Set<TMember>(Expression<Func<TPropModel, TMember>> selector, TMember value)
        {
            /**
             * (model, v) => model.XXX = v;
             */
            var prop = selector.ExtractProperty();
            var action = propAssignCaches.GetOrAdd((typeof(TPropModel), prop), key =>
            {
                var modelExp = Expression.Parameter(key.Entity);
                var p = Expression.Parameter(key.Prop.PropertyType);
                return Expression.Lambda<Action<TPropModel, TMember>>(Expression.Assign(Expression.Property(modelExp, key.Prop), p), modelExp, p).Compile();
            });
            action.DynamicInvoke(Model, value);
            return this;
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
        //public IBindableInputComponent<TPropModel, TValue> Bind(Expression<Func<TValue>> expression, string valueName, Func<Task>? onchange = null)
        //{

        //}

        public override RenderFragment Render()
        {
            tpropHandle?.Invoke((this as TSelf)!);
            return newRender?.Invoke((this as TSelf)!) ?? base.Render();
        }

    }
    [IgnoreAutoInject]
    public class BindableInputComponentBuilder<TComponent, TPropModel, TValue> : BindableInputComponentBuilder<TComponent, TPropModel, TValue, BindableInputComponentBuilder<TComponent, TPropModel, TValue>>, IBindableInputComponent<TPropModel, TValue>
        where TComponent : IComponent
        where TPropModel : DefaultProp, new()
    {
        public BindableInputComponentBuilder()
        {

        }

        public BindableInputComponentBuilder(Func<BindableInputComponentBuilder<TComponent, TPropModel, TValue>, RenderFragment> newRender)
        {
            this.newRender = newRender;
        }

        public BindableInputComponentBuilder(Action<BindableInputComponentBuilder<TComponent, TPropModel, TValue>> tpropHandle)
        {
            this.tpropHandle = tpropHandle;
        }
    }
}
