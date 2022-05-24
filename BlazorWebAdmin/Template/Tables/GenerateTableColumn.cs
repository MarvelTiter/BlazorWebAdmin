using BlazorWebAdmin.Template.Tables.Setting;
using Project.Common.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BlazorWebAdmin.Template.Tables
{
    public static class GenerateTableColumn
    {
        public static List<ColumnDefinition> GenerateColumns(this Type self)
        {
            var props = self.GetProperties();
            var heads = props.Where(p => p.GetCustomAttribute<ColumnDefinitionAttribute>() != null);
            List<ColumnDefinition> columns = new List<ColumnDefinition>();
            foreach (var col in heads)
            {
                var head = col.GetCustomAttribute<ColumnDefinitionAttribute>();
                ColumnDefinition column = new(head!.Label, col.Name)
                {
                    Index = head.Sort,
                    DataType = col.PropertyType,
                    Fixed = head.Fixed,
                    Width = head.Width,
                };
                if (column.IsEnum)
                {
                    column.EnumValues = ParseDictionary(col.PropertyType);
                }
                columns.Add(column);
            }
            columns.Sort((a, b) => a.Index - b.Index);
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
