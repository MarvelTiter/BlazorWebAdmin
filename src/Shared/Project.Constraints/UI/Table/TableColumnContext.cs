using MT.Toolkit.ReflectionExtension;
using Project.Constraints.Common;
using Project.Constraints.Common.Attributes;
using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Reflection;

namespace Project.Constraints.UI.Table
{
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
                var heads = props.Select(p => (Prop: p, Column: p.GetColumnDefinition()));
                List<ColumnInfo> columns = new List<ColumnInfo>();
                foreach (var col in heads)
                {
                    if (col.Column == null)
                    {
                        continue;
                    }
                    var column = col.Prop.GenerateColumn(col.Column);
                    column.ValueGetter = col.Prop.GetPropertyAccessor<object>();
                    column.ValueSetter = col.Prop.GetPropertySetter();
                    columns.Add(column);
                }
                if (columns.Any(c => c.Index != 0))
                {
                    columns.Sort((a, b) => a.Index - b.Index);
                }
                return new TableColumns(columns.ToArray());
            }) with
            { };
            return tc.Columns;
        }

        private static ColumnInfo GenerateColumn(this PropertyInfo self, ColumnDefinitionAttribute head)
        {
            if (head!.Label == null)
            {
                head!.Label = $"{self.DeclaringType!.Name}.{self.Name}";
            }
            ColumnInfo column = new(self)
            {
                Label = head.Label,
                Index = head.Sort,
                Fixed = head.Fixed,
                Width = head.Width,
                Visible = head.Visible,
                Ellipsis = head.Ellipsis,
                Readonly = head.Readonly,
                UseTag = head.UseTag,
                Sortable = head.Sortable
            };

            var formAttr = self.GetCustomAttribute<FormAttribute>();
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

            var colors = self.GetCustomAttributes<ColumnTagAttribute>();
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

        public static ColumnDefinitionAttribute? GetColumnDefinition(this PropertyInfo p)
        {
            var col = p.GetCustomAttribute<ColumnDefinitionAttribute>();
            if (col is null)
            {
                var dis = p.GetCustomAttribute<DisplayAttribute>();
                if (dis is not null)
                {
                    col = new ColumnDefinitionAttribute(dis.Name);
                }
            }

            if (p.GetCustomAttribute<FormAttribute>() is FormAttribute form && form != null)
            {
                // 没有 ColumnDefinitionAttribute 和 DisplayAttribute
                // 不在Table上显示，但是需要在表单上显示
                col ??= new ColumnDefinitionAttribute() { Visible = false };
            }

            return col;
        }

        public static Dictionary<string, string> ParseDictionary(Type enumType)
        {
            Dictionary<string, string> dict = new();
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
}
