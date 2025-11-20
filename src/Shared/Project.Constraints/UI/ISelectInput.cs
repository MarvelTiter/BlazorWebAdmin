using System.Linq.Expressions;

namespace Project.Constraints.UI;

/// <summary>
/// Select组件创建接口
/// </summary>
/// <typeparam name="TPropModel"></typeparam>
/// <typeparam name="TItem"></typeparam>
/// <typeparam name="TValue"></typeparam>
public interface ISelectInput<TPropModel, TItem, TValue> : IBindableInputComponent<TPropModel, TValue>
{
    IBindableInputComponent<TPropModel, TValue> Binds(Expression<Func<IEnumerable<TValue>>> expression);
    IBindableInputComponent<TPropModel, TValue> Binds(Expression<Func<IEnumerable<TValue>>> expression, Func<Task>? onchange);
}
