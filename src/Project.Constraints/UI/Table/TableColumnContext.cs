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
        public static List<ColumnInfo> GetColumnInfos<T>()
        {
            return typeof(T).GenerateColumns();
        }
        public static List<ColumnInfo> GenerateColumns(this Type type)
        {
            return StaticCache<List<ColumnInfo>>.GetOrAdd($"{type.FullName}_{type.GUID}", () =>
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
                    column.ValueGetter = CreateGetter(col.Prop);
                    column.ValueSetter = CreateSetter(col.Prop);
                    columns.Add(column);
                }
                //columns.Sort((a, b) => a.Index - b.Index);
                return [.. columns];
            });
        }

        private static Func<object, object>? CreateGetter(PropertyInfo prop)
        {
            /*
             * p => (object)p.XXX;
             */
            if (prop.DeclaringType == null || !prop.CanRead) return null;
            var p = Expression.Parameter(typeof(object), "p");
            var cp = Expression.Convert(p, prop.DeclaringType);
            var propExp = Expression.Property(cp, prop);
            var lambda = Expression.Lambda<Func<object, object>>(Expression.Convert(propExp, typeof(object)),p);
            return lambda.Compile();
        }

        private static Action<object, object>? CreateSetter(PropertyInfo prop)
        {
            /*
             * (p, v) => ((T)p).XXX = (TProp)v; 
             */
            if (prop.DeclaringType == null || !prop.CanWrite) return null;
            var p = Expression.Parameter(typeof(object), "p");
            var val = Expression.Parameter(typeof(object), "v");
            var cp = Expression.Convert(p, prop.DeclaringType);
            var setMethod = prop.SetMethod!;
            var set = Expression.Call(cp, setMethod, Expression.Convert(val, prop.PropertyType));
            var lambda = Expression.Lambda<Action<object, object>>(set, p, val);
            return lambda.Compile();
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
