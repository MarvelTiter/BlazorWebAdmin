using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Project.Common.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ColumnDefinitionAttribute : Attribute
    {
        public ColumnDefinitionAttribute(int sort = 0, string? fix = null, string? width = null) : this(null, sort, fix, width)
        {

        }
        public ColumnDefinitionAttribute(string? label, int sort = 0, string? fix = null, string? width = null)
        {
            Label = label;
            Sort = sort;
            Fixed = fix;
            Width = width;
        }
        public string? Label { get; set; }
        public int Sort { get; }
        public string? Fixed { get; }
        public string? Width { get; }
        /// <summary>
        /// 设置Tag颜色
        /// </summary>
        public bool UseTag { get; set; }
        public bool EnableEdit { get; set; } = false;
        public bool Visible { get; set; } = true;
        public bool Ellipsis { get; set; } = false;
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class ColumnTagAttribute : Attribute
    {
        public string Value { get; set; }
        public string Color { get; set; }
        public ColumnTagAttribute(object value, string color)
        {
            Color = color;
            if (value is string str)
            {
                Value = str;
            }
            else if (value is Enum e)
            {
                Value = value.GetType().GetField(e.ToString()!)?.GetCustomAttribute<DisplayAttribute>()?.Name ?? e.ToString();
            }
            else
            {
                Value = value?.ToString() ?? "";
            }
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class RoutePageAttribute : Attribute
    {
        public bool NoCache { get; set; }
    }
}
