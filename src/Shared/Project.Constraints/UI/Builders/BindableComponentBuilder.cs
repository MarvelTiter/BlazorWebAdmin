using Microsoft.AspNetCore.Components;
using MT.Toolkit.TypeConvertHelper;
using Project.Constraints.Common;
using Project.Constraints.UI.Props;
using System.Diagnostics;
using System.Linq.Expressions;

namespace Project.Constraints.UI.Builders;

public class BindableComponentBuilder<TComponent, TPropModel, TValue> : PropComponentBuilder<TComponent, TPropModel>, IBindableInputComponent<TPropModel, TValue>
    where TComponent : IComponent
    where TPropModel : DefaultProp, new()
{

    protected TValue? value;
    protected EventCallback<TValue>? eventCallback;
    protected Action<TValue>? assignAction;
    protected Func<TValue, Task>? callback;

    protected string? stringValue;
    protected EventCallback<string>? stringEventCallback;
    protected Action<string>? stringAssignAction;
    protected Func<string, Task>? stringCallback;
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
        //if (Model.StringValue && typeof(TValue) != typeof(string))
        //{
        //    HandleStringValue(expression, onchange);
        //    return this;
        //}
        /*
         * () => context.Value;
         * Action<TValue> : context.Value = v;
         *                  await onchange.Invoke();
         */
        var body = expression.Body;
        var p = Expression.Parameter(typeof(TValue), "v");
        var actionExp = Expression.Lambda<Action<TValue>>(Expression.Assign(body, p), p);
        assignAction = actionExp.Compile();
        callback = v =>
        {
            assignAction.Invoke(v);
            if (onchange != null)
                return onchange.Invoke();
            return Task.CompletedTask;
        };

        eventCallback = EventCallback.Factory.Create(Receiver, callback);
        var func = expression.Compile();
        value = func.Invoke();
        parameters.Add(Model.BindValueName, value!);
        parameters.Add($"{Model.BindValueName}Changed", eventCallback);
        if (Model.EnableValueExpression)
        {
            if (!Model.StringValue || typeof(TValue) == typeof(string))
            {
                parameters.Add("ValueExpression", expression);
            }
        }
        return this;
    }


    public void HandleStringValue(Expression<Func<TValue>> expression, Func<Task>? onchange)
    {
        var body = expression.Body;
        var p = Expression.Parameter(typeof(TValue), "v");
        var actionExp = Expression.Lambda<Action<TValue>>(Expression.Assign(body, p), p);
        assignAction = actionExp.Compile();
        stringCallback = v =>
        {
            Debug.WriteLine(v);
            var cv = v.ConvertTo<TValue>();
            assignAction.Invoke(cv!);
            if (onchange != null)
                return onchange.Invoke();
            return Task.CompletedTask;
        };

        stringEventCallback = EventCallback.Factory.Create(Receiver, stringCallback);
        var func = expression.Compile();
        stringValue = func.Invoke()?.ToString();
        parameters.Add(Model.BindValueName, stringValue);
        parameters.Add($"{Model.BindValueName}Changed", stringEventCallback);
        if (Model.EnableValueExpression)
        {
            //var s = Expression.Call(body, "ToString", [], []);
            //parameters.Add("ValueExpression", Expression.Lambda(s));
        }
    }
}