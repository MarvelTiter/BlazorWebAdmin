using BlazorTemplate.ClientCore.UI.Props;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace BlazorTemplate.ClientCore.UI.Builders;

public class SelectComponentBuilder<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TComponent, TPropModel, TItem, TValue> : BindableComponentBuilder<TComponent, TPropModel, TValue>, IBindableInputComponent<TPropModel, TValue>, ISelectInput<TPropModel, TItem, TValue>
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
            Type targetType = body.Type;

            var p = Expression.Parameter(typeof(IEnumerable<TValue>), "v");
            Expression assignExpression;
            if (targetType.IsAssignableFrom(typeof(IEnumerable<TValue>)))
            {
                assignExpression = Expression.Assign(body, p);
            }
            else
            {
                // 需要转换，例如：调用 ToList()、ToArray() 等
                assignExpression = CreateCollectionConversionExpression(body, p, targetType);
            }

            var actionExp = Expression.Lambda<Action<IEnumerable<TValue>>>(assignExpression, p);

            multiAssignAction = actionExp.Compile();

            multiCallback = v =>
            {
                v ??= [];
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

    private static readonly Func<IEnumerable<TValue>, TValue[]> toArray = Enumerable.ToArray;
    private static readonly Func<IEnumerable<TValue>, List<TValue>> toList = Enumerable.ToList;

    /// <summary>
    ///     为"集合改写回属性"构造赋值表达式。
    ///     <para>
    ///         表达式树侧用 <see cref="Expression.Invoke" /> 调用上面那两个委托实例，
    ///         替代原来的 <c>Expression.Call(MethodInfo, ...)</c>，
    ///         因此整条路径不再触碰 <c>MethodInfo</c>。
    ///     </para>
    /// </summary>
    private static BinaryExpression CreateCollectionConversionExpression(Expression target, Expression source, Type targetType)
    {
        // 根据目标类型选择合适的转换方法
        if (targetType.IsArray)
        {
            // 转换为数组
            return Expression.Assign(
                target,
                Expression.Convert(
                    Expression.Invoke(Expression.Constant(toArray), source),
                    targetType
                )
            );
        }
        else if (targetType.IsGenericType)
        {
            var genericTypeDef = targetType.GetGenericTypeDefinition();

            if (genericTypeDef == typeof(List<>))
            {
                // 转换为 List<T>
                return Expression.Assign(
                    target,
                    Expression.Convert(
                        Expression.Invoke(Expression.Constant(toList), source),
                        targetType
                    )
                );
            }
            else if (genericTypeDef == typeof(ICollection<>) ||
                     genericTypeDef == typeof(IList<>) ||
                     genericTypeDef == typeof(IEnumerable<>))
            {
                // 对于接口类型，使用适当的转换
                return Expression.Assign(
                    target,
                    Expression.Convert(source, targetType)
                );
            }
        }

        return Expression.Assign(target, Expression.Convert(source, targetType));
    }

}