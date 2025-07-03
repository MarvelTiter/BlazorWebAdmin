﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Constraints.Common.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class AutoLoadJsModuleAttribute : Attribute
{
    /// <summary>
    /// 不包含js文件名称
    /// </summary>
    public string? Path { get; set; }
}