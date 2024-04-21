using Microsoft.AspNetCore.Components;
using Project.Constraints.Common.Attributes;
using System.Reflection;

namespace Project.Constraints.UI.Table;


[IgnoreAutoInject]
public record ColumnInfo(PropertyInfo Property)
{
    public string Label { get; set; }
    public string PropertyOrFieldName => Property.Name;
    public int Index { get; set; }
    public int? Row { get; set; }
    public int? Column { get; set; }
    public bool ShowOnForm { get; set; } = true;
    public Type DataType => Property.PropertyType;
    public bool IsEnum => DataType.IsEnum || (UnderlyingType?.IsEnum ?? false);
    public bool Nullable => (System.Nullable.GetUnderlyingType(DataType) ?? null) != null;
    public Type? UnderlyingType => System.Nullable.GetUnderlyingType(DataType);
    public string? Fixed { get; set; }
    public string? Width { get; set; }
    public bool Readonly { get; set; }
    public bool Ellipsis { get; set; }
    public bool Visible { get; set; } = true;
    public Func<object?, bool>? VisibleExpression { get; set; }
    public bool UseTag { get; set; }
    public Dictionary<string, string>? EnumValues { get; set; }
    public bool Sortable { get; set; }
    public Func<object, string>? ValueFormat { get; set; }
    public Dictionary<string, string> TagColors { get; set; }
    public Func<string, Dictionary<string, object>>? AddCellOptions { get; set; }
    public InputType? InputType { get; set; }
    public RenderFragment<object?>? CellTemplate { get; set; }
    public RenderFragment<FormItemContext>? FormTemplate { get; set; }
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
    public string GetTagColor(object? val)
    {
        if (TagColors?.TryGetValue(val?.ToString() ?? "", out var color) ?? false)
        {
            return color;
        }
        return "Blue";
    }
}

public class FormItemContext(object instance, ColumnInfo col)
{
    public object Instance { get; set; } = instance;
    public ColumnInfo Column { get; set; } = col;

    public object? GetValue() => Column.GetValue(Instance);

    public void SetValue(object val) => Column.SetValue(Instance, val);
}
