using Microsoft.AspNetCore.Components.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#pragma warning disable IDE0130
namespace Project.Web.Shared.Components;

public class SvgIcon : ComponentBase
{
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        //builder.OpenElement(0, HtmlTag);
        //if (OnClick.HasDelegate)
        //{
        //    builder.AddAttribute(1, "onclick", OnClick);
        //}

        //builder.AddContent(2, IconMarkuString);
        //builder.CloseComponent();
        builder.AddContent(0, IconMarkuString);
    }

    [Parameter] public string HtmlTag { get; set; } = "i";
    [Parameter] public string? IconName { get; set; }

    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object> AdditionalParameters { get; set; } = [];

    [Inject, NotNull] ISvgIconService? SvgService { get; set; }
    private MarkupString IconMarkuString;

    [Parameter] public string? ClassName { get; set; }

    [Parameter] public string? Style { get; set; }

    [Parameter] public EventCallback OnClick { get; set; }

    [Parameter] public string? FontSize { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        var result = await SvgService.GetIcon(IconName);
        LoadSvgData(result.Payload);
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
            var result = await SvgService.GetIcon(IconName);
            LoadSvgData(result.Payload);
            shouldUpdate = false;
        }
    }

    private void LoadSvgData(string? svgContent)
    {
        if (string.IsNullOrEmpty(svgContent))
        {
            IconMarkuString = new MarkupString($"<span class='iconfont {ClassName} icon-{IconName}'></span>");
        }
        else
        {
            //var style = Style ?? $"font-size: {FontSize ?? "18px"}";
            var insertIndex = svgContent.IndexOf("<svg") + 4;
            svgContent = svgContent.Insert(insertIndex, $" class='svg-icon {ClassName}' style='{Style}'");
            IconMarkuString = new MarkupString(svgContent);
        }
    }
}