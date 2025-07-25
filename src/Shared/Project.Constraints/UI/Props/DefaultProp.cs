﻿using Microsoft.AspNetCore.Components;

namespace Project.Constraints.UI.Props;

public class DefaultProp
{
    public string? Label { get; set; }
    public bool EnableValueExpression { get; set; } = true;
    public string BindValueName { get; set; } = "Value";
    public bool StringValue { get; set; } = false;
}

public class DatePickerProp: DefaultProp
{
    public bool WithTime { get; set; }
}