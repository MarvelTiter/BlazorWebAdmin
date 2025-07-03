using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Web.Shared.Components;

public partial class EdgeWidget : JsComponentBase
{
    [Parameter] public RenderFragment? ChildContent { get; set; }
    /// <summary>
    /// left / top /right /bottom
    /// </summary>
    [Parameter] public string Position { get; set; } = "left";

    ElementReference? maskDiv;
    ElementReference? containerDiv;
    ElementReference? triggerDiv;
    protected override async ValueTask Init()
    {
        await InvokeVoidAsync("init", new
        {
            mask = maskDiv,
            container = containerDiv,
            trigger = triggerDiv,
        });
    }
}