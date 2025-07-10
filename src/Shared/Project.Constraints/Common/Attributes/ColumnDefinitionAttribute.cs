using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Project.Constraints.Common.Attributes;

public enum InputType
{
    Text,
    Number,
    Boolean,
    DatePicker,
    Select,
    Password
}
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
    /// <summary>
    /// 列排序
    /// </summary>
    public int Sort { get; }
    /// <summary>
    /// 数据排序
    /// </summary>
    public bool Sortable { get; set; }
    public string? Fixed { get; }
    public string? Width { get; }
    public string? Align { get; set; }
    /// <summary>
    /// 设置Tag颜色
    /// </summary>
    public bool UseTag { get; set; }
    public bool Readonly { get; set; }
    public bool Visible { get; set; } = true;
    public bool Ellipsis { get; set; }
    public string? Format { get; set; }
    public bool Searchable { get; set; } = true;
}

[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
public class FormAttribute : Attribute
{
    public int Row { get; set; }
    public int Column { get; set; }
    public bool Hide { get; set; }
    public string? Label { get; set; }
    public InputType? InputType { get; set; }
    public FormAttribute()
    {

    }

    public FormAttribute(InputType inputType)
    {
        InputType = inputType;
    }
}

[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
public class ColumnTagAttribute : Attribute
{
    public string Value { get; set; }
    public string Color { get; set; }
    public ColumnTagAttribute(object formattedValue, string color)
    {
        Color = color;
        if (formattedValue is string str)
        {
            Value = str;
        }
        //else if (formattedValue is Enum e)
        //{
        //    Value = formattedValue.GetType().GetField(e.ToString()!)?.GetCustomAttribute<DisplayAttribute>()?.Name ?? e.ToString();
        //}
        else
        {
            Value = formattedValue?.ToString() ?? "";
        }
    }
}

[AttributeUsage(AttributeTargets.Class)]
public class RoutePageAttribute : Attribute
{
    public bool NoCache { get; set; }
}
