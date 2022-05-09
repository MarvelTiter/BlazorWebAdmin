using BlazorWebAdmin.Template.Tables.Setting;
using Project.Common.Attributes;
using System;
using System.Collections.Generic;
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
            var heads = props.Where(p => p.GetCustomAttribute<TableHeaderAttribute>() != null);
            List<ColumnDefinition> columns = new List<ColumnDefinition>();
            foreach (var col in heads)
            {
                var head = col.GetCustomAttribute<TableHeaderAttribute>();
                columns.Add(new(head!.Label, col.Name)
                {
                    Index = head.Sort,
                    DataType = col.PropertyType,
                    Fixed = head.Fixed,
                    Width = head.Width,
                });
            }
            columns.Sort((a, b) => a.Index - b.Index);
            return columns;
        }
    }
}
