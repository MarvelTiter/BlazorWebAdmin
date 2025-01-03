using Microsoft.AspNetCore.Components.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#pragma warning disable IDE0130
namespace Project.Web.Shared.Components;

public class MIcon : ComponentBase
{
    [Parameter]
    public string HtmlTag { get; set; } = "i";
    [Parameter]
    public string? IconName { get; set; }
    [Parameter]
    public string? ClassName { get; set; }
    [Parameter(CaptureUnmatchedValues = true)] public Dictionary<string, object> AdditionalParameters { get; set; } = [];

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (IconName?.StartsWith(AppConst.CUSTOM_SVG_PREFIX) == true)
        {
            builder.OpenComponent<SvgIcon>(0);
            builder.AddAttribute(1, nameof(SvgIcon.HtmlTag), HtmlTag);
            builder.AddAttribute(2, nameof(SvgIcon.IconName), IconName);
            builder.AddAttribute(3, nameof(SvgIcon.ClassName), ClassName);
            builder.AddAttribute(4, nameof(SvgIcon.AdditionalParameters), AdditionalParameters);
            builder.CloseComponent();
        }
        else if (IconName?.StartsWith("fa fa-") == true)
        {
            builder.OpenComponent<FontIcon>(0);
            builder.AddAttribute(1, nameof(FontIcon.HtmlTag), HtmlTag);
            builder.AddAttribute(2, nameof(FontIcon.IconName), IconName);
            builder.AddAttribute(3, nameof(FontIcon.ClassName), ClassName);
            builder.AddAttribute(4, nameof(FontIcon.AdditionalParameters), AdditionalParameters);
            builder.CloseComponent();
        }
    }
}

