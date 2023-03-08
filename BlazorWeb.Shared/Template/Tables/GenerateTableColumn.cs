using BlazorWeb.Shared.Template.Tables.Setting;
using MDbEntity.Attributes;
using Project.Common.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BlazorWeb.Shared.Template.Tables
{
    public static class GenerateTableColumn
    {
        public static TableOptionColumn GenerateColumn(this PropertyInfo self)
        {
            var head = self.GetCustomAttribute<ColumnDefinitionAttribute>();
            var dbInfo = self.GetCustomAttribute<ColumnAttribute>();
            TableOptionColumn column = new(head!.Label, self.Name)
            {
                Index = head.Sort,
                DataType = self.PropertyType,
                Fixed = head.Fixed,
                Width = head.Width,
                Visible = head.Visible,
                Ellipsis = head.Ellipsis,
                EnableEdit = head.EnableEdit,
            };
            if (column.IsEnum)
            {
                column.EnumValues = ParseDictionary(self.PropertyType);
            }
            return column;
        }
        public static List<TableOptionColumn> GenerateColumns(this Type self)
        {
            var props = self.GetProperties();
            var heads = props.Where(p => p.GetCustomAttribute<ColumnDefinitionAttribute>() != null);
            List<TableOptionColumn> columns = new List<TableOptionColumn>();
            foreach (var col in heads)
            {
                var column = col.GenerateColumn();
                columns.Add(column);
            }
            //columns.Sort((a, b) => a.Index - b.Index);
            return columns;
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
