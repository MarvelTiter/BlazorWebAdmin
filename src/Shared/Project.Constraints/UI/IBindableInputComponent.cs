using System.Linq.Expressions;

namespace Project.Constraints.UI;

/// <summary>
/// 支持 @bind-XXXX 操作的组件创建接口
/// </summary>
/// <typeparam name="TPropModel"></typeparam>
/// <typeparam name="TValue"></typeparam>
public interface IBindableInputComponent<TPropModel, TValue> : IUIComponent<TPropModel>
{
    IBindableInputComponent<TPropModel, TValue> Bind(Expression<Func<TValue>> expression);
    IBindableInputComponent<TPropModel, TValue> Bind(Expression<Func<TValue>> expression, Func<Task>? onchange);
    //IBindableInputComponent<TPropModel, TValue> Bind(Expression<Func<TValue>> expression, string valueName, Func<Task>? onchange = null);
}
