﻿using System;

namespace Project.Constraints.Aop;

[AttributeUsage(AttributeTargets.Method)]
public class LogInfoAttribute : Attribute
{
	public string? Module { get; set; }
	public string? Action { get; set; }
}