namespace Project.Constraints.Common.Attributes;

/// <summary>
/// 接口属性的<see cref="ColumnDefinitionAttribute"/>适用于子类, 子类依然可以定义<see cref="ColumnDefinitionAttribute"/>覆盖
/// </summary>
[AttributeUsage(AttributeTargets.Interface)]
public class SupplyColumnDefinition : Attribute
{

}