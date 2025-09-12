using Microsoft.AspNetCore.Components.Rendering;
using Project.Constraints.Common.Attributes;
using Project.Constraints.PageHelper;
using Project.Constraints.Store.Models;
using Project.Constraints.UI.Extensions;
using Project.Web.Shared.Components;
using Project.Web.Shared.Pages;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Project.Web.Shared.Layouts.LayoutComponents;

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

        if (RouterStore.Current?.Panic == true)
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
