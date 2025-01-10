using Microsoft.AspNetCore.Components.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#pragma warning disable IDE0130
namespace Project.Web.Shared.Components;

public class FontIcon : ComponentBase
{
    [Parameter]
    public string HtmlTag { get; set; } = "i";
    [Parameter]
    public string? IconName { get; set; }
    [Parameter]
    public string? ClassName { get; set; }
    [Parameter(CaptureUnmatchedValues = true)] public Dictionary<string, object> AdditionalParameters { get; set; } = [];
    string IconClass => $"{IconName} {ClassName}";
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, HtmlTag);
        builder.AddAttribute(1, "class", IconClass);
        builder.AddMultipleAttributes(2, AdditionalParameters);
        builder.CloseElement();
    }
}
