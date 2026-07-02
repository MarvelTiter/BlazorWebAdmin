using Microsoft.AspNetCore.Components;
using Project.Constraints.Common.Attributes;
using System.Linq.Expressions;
using System.Reflection;

namespace Project.Constraints.UI.Table;

public interface IColumnInfo
{
    [NotNull] string? Label { get; set; }
    string PropertyOrFieldName { get; }
    int Index { get; set; }
    int? Row { get; set; }
    int? Column { get; set; }
    int ColumnIndex { get; set; }
    bool ShowOnForm { get; set; }
    Type DataType { get; }
    bool IsEnum { get; }
    bool Nullable { get; }
    Type? UnderlyingType { get; }
    string? Fixed { get; set; }
    string? Width { get; set; }
    string? Align { get; set; }
    bool Readonly { get; set; }
    bool Ellipsis { get; set; }
    bool Editable { get; set; }
    bool Visible { get; set; }
    bool Searchable { get; set; }
    Func<object?, bool>? VisibleExpression { get; set; }
    bool UseTag { get; set; }
    bool Sortable { get; set; }
    Dictionary<string, string>? EnumValues { get; set; }
    Dictionary<string, string>? TagColors { get; set; }
    InputType? InputType { get; set; }
    Func<object, string>? ValueFormat { get; set; }
    string? Format { get; set; }
    Func<string, Dictionary<string, object>>? AddCellOptions { get; set; }
    RenderFragment<ColumnItemContext>? CellTemplate { get; set; }
    RenderFragment<ColumnItemContext>? FormTemplate { get; set; }
    bool Grouping { get; set; }
    Func<object, object> GroupByExpression { get; set; }
    Expression? MemberAccessExpression { get; set; }
    Action<object, object>? ValueSetter { get; set; }
    Func<object, object>? ValueGetter { get; set; }

    object? GetValue(object target);
    void SetValue(object target, object val);
    object MemberwiseClone();
}

public record ColumnInfo : IColumnInfo
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
    public new object MemberwiseClone() => base.MemberwiseClone();
    [NotNull] public string? Label { get; set; }
    public string PropertyOrFieldName { get; }
    /// <summary>
    /// 排序使用
    /// </summary>
    public int Index { get; set; }
    /// <summary>
    /// 表单布局使用
    /// </summary>
    public int? Row { get; set; }
    /// <summary>
    /// 表单布局使用
    /// </summary>
    public int? Column { get; set; }
    /// <summary>
    /// 列索引
    /// </summary>
    public int ColumnIndex { get; set; }
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
    public bool Editable { get; set; }
    public bool Visible { get; set; } = true;
    /// <summary>
    /// 是否可作为查询条件
    /// </summary>
    public bool Searchable { get; set; } = true;
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
    public Action<object, object>? ValueSetter { get; set; }
    public Func<object, object>? ValueGetter { get; set; }
    public object? GetValue(object target) => ValueGetter?.Invoke(target);
    public void SetValue(object target, object val) => ValueSetter?.Invoke(target, val);
    public Func<object, object> GroupByExpression
    {
        get => groupByExpression;
        set
        {
            groupByExpression = value;
            Grouping = value != null;
        }
    }
    public Expression? MemberAccessExpression { get; set; }
}

public static class ColumnInfoExtensions
{
    public static string GetTagColor(this IColumnInfo column, object? val)
    {
        if (column.TagColors?.TryGetValue(val?.ToString() ?? "", out var color) ?? false)
        {
            return color;
        }
        return "Blue";
    }

    public static Expression<Func<TData, TProp>> MakeExpression<TData, TProp>(this IColumnInfo col)
    {
        col.MemberAccessExpression ??= Build();

        return (Expression<Func<TData, TProp>>)col.MemberAccessExpression;

        Expression<Func<TData, TProp>> Build()
        {
            var p = Expression.Parameter(typeof(TData), "p");
            return Expression.Lambda<Func<TData, TProp>>(Expression.Property(p, col.PropertyOrFieldName), p);
        }
    }
}
