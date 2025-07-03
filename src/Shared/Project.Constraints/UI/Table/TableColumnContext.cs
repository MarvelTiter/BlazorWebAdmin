using MT.Toolkit.ReflectionExtension;
using Project.Constraints.Common;
using Project.Constraints.Common.Attributes;
using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Reflection;

namespace Project.Constraints.UI.Table;

public static class TableColumnContext
{
    public record TableColumns(ColumnInfo[] Columns);
    public static ColumnInfo[] GetColumnInfos<T>()
    {
        return typeof(T).GenerateColumns();
    }
    public static ColumnInfo[] GenerateColumns(this Type type)
    {
        var tc = StaticCache<TableColumns>.GetOrAdd($"{type.FullName}_{type.GUID}", () =>
            {
                var props = type.GetProperties();
                PropertyInfo[] interfaceDefProps = [.. type.GetInterfaces().Where(i => i.GetCustomAttribute<SupplyColumnDefinition>() is not null).SelectMany(i => i.GetProperties())];
                //var heads = props.Select(p => (Prop: p, Column: p.GetColumnDefinition()));
                List<ColumnInfo> columns = [];
                foreach (var prop in props)
                {
                    var upper = interfaceDefProps.FirstOrDefault(p => p.Name == prop.Name);
                    var definition = GetColumnDefinition(prop, upper);
                    if (definition is null)
                    {
                        continue;
                    }
                    var column = GenerateColumn(prop, definition, upper);
                    column.ValueGetter = prop.GetPropertyAccessor<object>();
                    column.ValueSetter = prop.GetPropertySetter();
                    columns.Add(column);
                }
                if (columns.Any(c => c.Index != 0))
                {
                    columns.Sort((a, b) => a.Index - b.Index);
                }
                return new TableColumns([.. columns]);
            }) with
        { };
        return tc.Columns;
    }

    private static ColumnInfo GenerateColumn(PropertyInfo self, ColumnDefinitionAttribute head, PropertyInfo? upper)
    {
        if (head.Label == null)
        {
            var lang = self.DeclaringType?.GetCustomAttribute<LangNameAttribute>() ?? upper?.DeclaringType?.GetCustomAttribute<LangNameAttribute>();
            if (lang is not null)
            {
                head.Label = $"{lang.Name}.{self.Name}";
            }
            else
            {
                head.Label = $"{self.DeclaringType!.Name}.{self.Name}";
            }
        }
        ColumnInfo column = new(self)
        {
            Label = head.Label,
            Index = head.Sort,
            Fixed = head.Fixed,
            Width = head.Width,
            Align = head.Align,
            Visible = head.Visible,
            Ellipsis = head.Ellipsis,
            Readonly = head.Readonly,
            UseTag = head.UseTag,
            Sortable = head.Sortable,
            Format = head.Format
        };

        var formAttr = self.GetCustomAttribute<FormAttribute>() ?? upper?.GetCustomAttribute<FormAttribute>();
        if (formAttr != null)
        {
            column.Row = formAttr.Row;
            column.Column = formAttr.Column;
            column.ShowOnForm = !formAttr.Hide;
            column.InputType = formAttr.InputType;
            if (!string.IsNullOrEmpty(formAttr.Label))
            {
                column.Label = formAttr.Label;
            }
        }

        if (column.IsEnum)
        {
            column.EnumValues = ParseDictionary(column.UnderlyingType ?? column.DataType);
        }

        var colors = self.GetCustomAttributes<ColumnTagAttribute>().Concat(upper?.GetCustomAttributes<ColumnTagAttribute>() ?? []);
        if (colors?.Any() ?? false)
        {
            column.UseTag = true;
            column.TagColors = [];
            foreach (var c in colors)
            {
                if (c == null) continue;
                column.TagColors.TryAdd(c.Value, c.Color);
            }
        }
        return column;
    }

    public static ColumnDefinitionAttribute? GetColumnDefinition(PropertyInfo p, PropertyInfo? upper)
    {
        var col = p.GetCustomAttribute<ColumnDefinitionAttribute>() ?? upper?.GetCustomAttribute<ColumnDefinitionAttribute>();
        if (col is null)
        {
            var dis = p.GetCustomAttribute<DisplayAttribute>() ?? upper?.GetCustomAttribute<DisplayAttribute>();
            if (dis is not null)
            {
                col = new ColumnDefinitionAttribute(dis.Name);
            }
        }
        FormAttribute? form = p.GetCustomAttribute<FormAttribute>() ?? upper?.GetCustomAttribute<FormAttribute>();
        if (form != null)
        {
            // 没有 ColumnDefinitionAttribute 和 DisplayAttribute
            // 不在Table上显示，但是需要在表单上显示
            col ??= new ColumnDefinitionAttribute() { Visible = false };
        }

        return col;
    }

    public static Dictionary<string, string> ParseDictionary(Type enumType)
    {
        Dictionary<string, string> dict = [];
        foreach (var member in Enum.GetValues(enumType).Cast<Enum>())
        {
            var name = Enum.GetName(enumType, member)!;
            var mem = enumType.GetMember(name)[0];
            var label = mem.GetCustomAttribute<DisplayAttribute>()?.Name ?? name;
            dict.Add(name, label!);
        }
        return dict;
    }
}