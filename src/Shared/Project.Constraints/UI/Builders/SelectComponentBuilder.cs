using Microsoft.AspNetCore.Components;
using Project.Constraints.UI.Props;
using System.Linq.Expressions;

namespace Project.Constraints.UI.Builders;

public class SelectComponentBuilder<TComponent, TPropModel, TItem, TValue> : BindableComponentBuilder<TComponent, TPropModel, TValue>, IBindableInputComponent<TPropModel, TValue>, ISelectInput<TPropModel, TItem, TValue>
    where TComponent : IComponent
    where TPropModel : DefaultProp, new()
{
    public SelectComponentBuilder()
    {

    }

    public SelectComponentBuilder(Func<PropComponentBuilder<TComponent, TPropModel>, RenderFragment> newRender)
    {
        this.newRender = newRender;
    }

    public SelectComponentBuilder(Action<PropComponentBuilder<TComponent, TPropModel>> tpropHandle)
    {
        this.tpropHandle = tpropHandle;
    }

    public IBindableInputComponent<TPropModel, TValue> Binds(Expression<Func<IEnumerable<TValue>>> expression)
    {
        return Binds(expression, null);
    }

    protected IEnumerable<TValue>? values;
    protected EventCallback<IEnumerable<TValue>>? multiEventCallback;
    protected Action<IEnumerable<TValue>>? multiAssignAction;
    protected Func<IEnumerable<TValue>, Task>? multiCallback;

    Expression<Func<IEnumerable<TValue>>>? expression;
    public IBindableInputComponent<TPropModel, TValue> Binds(Expression<Func<IEnumerable<TValue>>> expression, Func<Task>? onchange)
    {
        this.expression = expression;
        this.onchange = onchange;
        handleBind = () =>
        {
            var body = this.expression.Body;
            var p = Expression.Parameter(typeof(IEnumerable<TValue>), "v");
            var actionExp = Expression.Lambda<Action<IEnumerable<TValue>>>(Expression.Assign(body, p), p);
            multiAssignAction = actionExp.Compile();
            multiCallback = v =>
            {
                multiAssignAction.Invoke(v);
                if (this.onchange != null)
                    return this.onchange.Invoke();
                return Task.CompletedTask;
            };

            multiEventCallback = EventCallback.Factory.Create(Receiver, multiCallback);
            var func = this.expression.Compile();
            values = func.Invoke();
            parameters.Add(Model.BindValueName, values!);
            parameters.Add($"{Model.BindValueName}Changed", multiEventCallback);
            if (Model.EnableValueExpression)
            {
                if (!Model.StringValue || typeof(TValue) == typeof(string))
                {
                    parameters.Add(Model.ValueExpressionName, this.expression);
                }
            }
        };
        return this;
    }
}