using Microsoft.AspNetCore.Components;
using Project.Constraints.Common.Attributes;
using System.Reflection;

namespace Project.Constraints.UI.Table;
public record ColumnInfo
{
    public ColumnInfo(string label, string propertyName)
    {
        DataType = typeof(string);
        Label = label;
        PropertyOrFieldName = propertyName;
    }
    public ColumnInfo(PropertyInfo Property)
    {
        PropertyOrFieldName = Property.Name;
        DataType = Property.PropertyType;
    }

    [NotNull] public string? Label { get; set; }
    public string PropertyOrFieldName { get; }
    public int Index { get; set; }
    public int? Row { get; set; }
    public int? Column { get; set; }
    public bool ShowOnForm { get; set; } = true;
    public Type DataType { get; }
    public bool IsEnum => DataType.IsEnum || (UnderlyingType?.IsEnum ?? false);
    public bool Nullable => System.Nullable.GetUnderlyingType(DataType) != null;
    public Type? UnderlyingType => System.Nullable.GetUnderlyingType(DataType);
    public string? Fixed { get; set; }
    public string? Width { get; set; }
    public string? Align { get; set; }
    public bool Readonly { get; set; }
    public bool Ellipsis { get; set; }
    public bool Visible { get; set; } = true;
    /// <summary>
    /// 参数是行数据对象
    /// </summary>
    public Func<object?, bool>? VisibleExpression { get; set; }
    public bool UseTag { get; set; }
    public bool Sortable { get; set; }
    public Dictionary<string, string>? EnumValues { get; set; }
    public Dictionary<string, string>? TagColors { get; set; }
    public InputType? InputType { get; set; }
    public Func<object, string>? ValueFormat { get; set; }
    public string? Format { get; set; }
    public Func<string, Dictionary<string, object>>? AddCellOptions { get; set; }
    public RenderFragment<ColumnItemContext>? CellTemplate { get; set; }
    public RenderFragment<ColumnItemContext>? FormTemplate { get; set; }
    public bool Grouping { get; set; }

    private Func<object, object> groupByExpression = static obj => 0;
    internal Action<object, object>? ValueSetter { get; set; }
    internal Func<object, object>? ValueGetter { get; set; }
    public object? GetValue(object target) => ValueGetter?.Invoke(target);
    public void SetValue(object target, object val) => ValueSetter?.Invoke(target, val);
    public Func<object, object> GroupByExpression
    {
        get => groupByExpression;
        set
        {
            groupByExpression = value;
            Grouping = true;
        }
    }

}

public static class ColumnInfoExtensions
{
    public static string GetTagColor(this ColumnInfo column, object? val)
    {
        if (column.TagColors?.TryGetValue(val?.ToString() ?? "", out var color) ?? false)
        {
            return color;
        }
        return "Blue";
    }
}
