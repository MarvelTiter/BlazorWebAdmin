using Project.Constraints.UI.Props;

namespace Project.Constraints.UI;

/// <summary>
/// 按钮组件创建接口
/// </summary>
public interface IButtonInput : IUIComponent<ButtonProp>, IClickable<IButtonInput>;
