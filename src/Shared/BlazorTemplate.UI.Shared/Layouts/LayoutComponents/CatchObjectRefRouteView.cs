using Microsoft.AspNetCore.Components.Rendering;
using BlazorTemplate.Constraints.UI.Extensions;
using BlazorTemplate.UI.Shared.Components;
using BlazorTemplate.UI.Shared.Pages;
using System.Collections.Concurrent;
using System.Reflection;

namespace BlazorTemplate.UI.Shared.Layouts.LayoutComponents;

public class CatchObjectRefRouteView : RouteView
{
    [Inject, NotNull] private IRouterStore? RouterStore { get; set; }

    private static readonly ConcurrentDictionary<Type, Type?> _layoutAttributeCache = new();

    protected override void Render(RenderTreeBuilder builder)
    {
        var pageLayoutType = _layoutAttributeCache
                                 .GetOrAdd(RouteData.PageType, static type => type.GetCustomAttribute<LayoutAttribute>()?.LayoutType)
                             ?? DefaultLayout;

        builder.OpenComponent<LayoutView>(0);
        builder.AddComponentParameter(1, nameof(LayoutView.Layout), pageLayoutType);
        builder.AddComponentParameter(2, nameof(LayoutView.ChildContent), (RenderFragment)RenderPageWithParameters);
        builder.CloseComponent();
    }

    private void RenderPageWithParameters(RenderTreeBuilder builder)
    {
        if (RouterStore.RouteChanging)
        {
            return;
        }

        if (!RouterStore.LastRouterChangingCheck)
        {
            builder.Component<NotAuthorizedPage>().Build();
            
            return;
        }

        if (RouterStore.Current?.Exception is not null && RouterStore.Current?.Panic == true)
        {
            builder.Component<CrashPage>()
                .SetComponent(c => c.Exception, RouterStore.Current.Exception)
                .Build();
            return;
        }

        builder.OpenComponent(0, RouteData.PageType);
        foreach (var kvp in RouteData.RouteValues)
        {
            builder.AddComponentParameter(1, kvp.Key, kvp.Value);
        }

        builder.AddComponentReferenceCapture(2, RouterStore.CollectPageAdditionalInfo);
        builder.CloseComponent();
    }
}
