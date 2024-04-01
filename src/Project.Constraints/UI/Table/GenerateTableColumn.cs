using Project.Constraints.Common.Attributes;
using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Project.Constraints.UI.Table
{
    public static class GenerateTableColumn
    {
        static readonly ConcurrentDictionary<Type, List<ColumnInfo>> caches = new();

        public static ColumnInfo GenerateColumn(this PropertyInfo self, ColumnDefinitionAttribute head)
        {
            //var head = self.GetCustomAttribute<ColumnDefinitionAttribute>();
            //if (head == null)
            //{
            //    var dis = self.GetCustomAttribute<DisplayAttribute>();
            //    if (dis is not null)
            //    {
            //        head = new ColumnDefinitionAttribute(dis.Name);
            //    }
            //}

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
        public static List<ColumnInfo> GenerateColumns(this Type self)
        {
            return caches.GetOrAdd(self, type =>
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
                     columns.Add(column);
                 }
                 //columns.Sort((a, b) => a.Index - b.Index);
                 return [.. columns];
             });
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
