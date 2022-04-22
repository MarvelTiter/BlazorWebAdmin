using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Common.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class TableHeaderAttribute : Attribute
    {
        public TableHeaderAttribute(string label, int sort, string? fix = null, string width = null)
        {
            Label = label;
            Sort = sort;
            Fixed = fix;
            Width = width;
        }
        public string Label { get; }
        public int Sort { get; }
        public string? Fixed { get; }
        public string? Width { get; }
    }
}
