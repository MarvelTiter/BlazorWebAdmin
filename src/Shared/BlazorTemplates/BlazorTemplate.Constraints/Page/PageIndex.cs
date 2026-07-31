using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using BlazorTemplate.Constraints.PageHelper;
using BlazorTemplate.Constraints.Services;

namespace BlazorTemplate.Constraints.Page;

/// <summary>
/// 自定义页面
/// </summary>
[Obsolete("使用SystemPageIndex")]
public abstract class PageIndex : ComponentBase, IRoutePage
{
    [Inject, NotNull] IPageLocatorService? PageLocator { get; set; }
    protected abstract Type? GetPageType(IPageLocatorService pageLocator);
    IRoutePage? page;
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        var type = GetPageType(PageLocator);
        if (type != null)
        {
            builder.OpenComponent(0, type);
            builder.AddComponentReferenceCapture(1, obj =>
            {
                page = obj as IRoutePage;
            });
            builder.CloseComponent();
        }
    }

    public void OnClose() => page?.OnClose();
}
