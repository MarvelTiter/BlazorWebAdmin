using Project.Common.Attributes;
using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Project.Constraints.UI.Table
{
    public static class GenerateTableColumn
    {
        static readonly ConcurrentDictionary<Type, List<ColumnInfo>> caches = new();
        public static ColumnInfo GenerateColumn(this PropertyInfo self)
        {
            var head = self.GetCustomAttribute<ColumnDefinitionAttribute>() ?? throw new ArgumentException();
            return self.GenerateColumn(head);
        }
        public static ColumnInfo GenerateColumn(this PropertyInfo self, ColumnDefinitionAttribute head)
        {
            //var head = self.GetCustomAttribute<ColumnDefinitionAttribute>();
            //var dbInfo = self.GetCustomAttribute<ColumnAttribute>();
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
                EnableEdit = head.EnableEdit,
                UseTag = head.UseTag,
                Sortable = head.Sortable
            };
            if (column.IsEnum)
            {
                column.EnumValues = ParseDictionary(column.UnderlyingType ?? column.DataType);
            }
            if (column.UseTag)
            {
                var colors = self.GetCustomAttributes<ColumnTagAttribute>();
                if (colors?.Count() > 0)
                {
                    column.TagColors = new();
                    foreach (var c in colors)
                    {
                        if (c == null) continue;
                        column.TagColors.TryAdd(c.Value, c.Color);
                    }
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
                 return columns;
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
