using Microsoft.AspNetCore.Components.Rendering;
using Project.Constraints.UI.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#pragma warning disable IDE0130
namespace Project.Web.Shared.Components;

public class SvgIcon : ComponentBase
{
    //private MarkupString IconMarkuString;
    private SvgParsingResult? svgData;
    [Obsolete]
    [Parameter] public string HtmlTag { get; set; } = "i";
    [Parameter] public string? IconName { get; set; }

    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object> AdditionalParameters { get; set; } = [];

    [Inject, NotNull] ISvgIconService? SvgService { get; set; }

    [Parameter] public string? ClassName { get; set; }

    [Parameter] public string? Style { get; set; }

    [Parameter] public EventCallback OnClick { get; set; }

    [Parameter] public string? FontSize { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        var result = await SvgService.GetIconAsync(IconName);
        //LoadSvgData(result.Payload?.OriginalContent);
        svgData = result.Payload;
    }

    public override async Task SetParametersAsync(ParameterView parameters)
    {
        if (parameters.TryGetValue(nameof(IconName), out string? result))
        {
            if (!string.Equals(IconName, result)
                && !string.IsNullOrEmpty(IconName)
                && !string.IsNullOrEmpty(result))
            {
                shouldUpdate = true;
            }
        }

        await base.SetParametersAsync(parameters);
    }

    bool shouldUpdate;

    protected override async Task OnParametersSetAsync()
    {
        // await loadSvgData();
        await base.OnParametersSetAsync();
        if (shouldUpdate)
        {
            var result = await SvgService.GetIconAsync(IconName);
            //LoadSvgData(result.Payload?.OriginalContent);
            svgData = result.Payload;
            shouldUpdate = false;
        }
    }

    //private void LoadSvgData(string? svgContent)
    //{
    //    if (string.IsNullOrEmpty(svgContent))
    //    {
    //        IconMarkuString = new MarkupString($"<span class='iconfont {ClassName} icon-{IconName}'></span>");
    //    }
    //    else
    //    {
    //        //var style = Style ?? $"font-size: {FontSize ?? "18px"}";
    //        var insertIndex = svgContent.IndexOf("<svg") + 4;
    //        svgContent = svgContent.Insert(insertIndex, $" class='svg-icon {ClassName}' style='{Style}'");
    //        IconMarkuString = new MarkupString(svgContent);
    //    }
    //}

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (svgData is not null)
        {
            builder.OpenElement(0, "svg");
            builder.AddMultipleAttributes(1, svgData.Attributes);
            if (OnClick.HasDelegate)
            {
                builder.AddAttribute(2, "onclick", OnClick);
                //builder.AddAttribute(2, "style", "cursor:pointer");
            }
            builder.AddAttribute(3, "class", $"svg-icon {ClassName}");
            if (!string.IsNullOrEmpty(Style))
                builder.AddAttribute(4, "style", Style);
            builder.AddMarkupContent(5, svgData.InnerContent);
            builder.CloseElement();
        }
    }
}