using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace BlazorAdmin;

public class Routers : ComponentBase
{
    [Parameter] public bool NeedAuthentication { get; set; }
    [Parameter] public IComponentRenderMode? RenderMode { get; set; }
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        //if (NeedAuthentication)
        //{
#if DEBUG
        builder.OpenComponent<BlazorAdmin.Client.Routes>(0);
        builder.AddComponentRenderMode(RenderMode);
        builder.CloseComponent();
#else
#if (UseClientProject)
        builder.OpenComponent<BlazorAdmin.Client.Routes>(0);
        builder.AddComponentRenderMode(RenderMode);
        builder.CloseComponent();
#endif
#if (!UseClientProject)
        builder.OpenComponent<BlazorAdmin.ServerRoutes>(0);
        builder.AddComponentRenderMode(RenderMode);
        builder.CloseComponent();
#endif
#endif
        //        }
        //        else
        //        {
        //#if DEBUG
        //            builder.OpenComponent<BlazorAdmin.Client.FreeRoutes>(0);
        //            builder.AddComponentRenderMode(RenderMode);
        //            builder.CloseComponent();
        //#else
        //#if (UseClientProject)
        //            builder.OpenComponent<BlazorAdmin.Client.FreeRoutes>(0);
        //            builder.AddComponentRenderMode(RenderMode);
        //            builder.CloseComponent();
        //#endif
        //#if (!UseClientProject)
        //            builder.OpenComponent<BlazorAdmin.ServerFreeRoutes>(0);
        //            builder.AddComponentRenderMode(RenderMode);
        //            builder.CloseComponent();
        //#endif
        //#endif
        //        }
    }
}
