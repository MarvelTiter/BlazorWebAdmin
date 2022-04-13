﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Common.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class TableHeaderAttribute : Attribute
    {
        public TableHeaderAttribute(string label, int sort)
        {
            Label = label;
            Sort = sort;
        }
        public string Label { get; }
        public int Sort { get; }
    }
}
