using Microsoft.AspNetCore.Components;
using System.Linq.Expressions;

namespace Project.Constraints.UI;

/// <summary>
/// 组件创建接口
/// </summary>
public interface IUIComponent
{
    IUIComponent Set(string key, object value);
    IUIComponent Style(string value);
    IUIComponent SetIf(bool condition, string key, object value);
    IUIComponent AdditionalParameters(Dictionary<string, object> parameters);
    RenderFragment Render();
}

/// <summary>
/// 带有属性模型的组件创建接口
/// </summary>
/// <typeparam name="TPropModel">属性模型</typeparam>
public interface IUIComponent<TPropModel> : IUIComponent
{
    TPropModel Model { get; set; }
    IUIComponent<TPropModel> Set<TMember>(Expression<Func<TPropModel, TMember>> selector, TMember value);
    IUIComponent<TPropModel> SetModel(Action<TPropModel> action);
}
