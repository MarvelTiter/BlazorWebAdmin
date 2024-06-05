using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Project.Constraints.PageHelper;
using Project.Constraints.Services;
using System.Diagnostics.CodeAnalysis;

namespace Project.Constraints.Page;

/// <summary>
/// 自定义页面
/// </summary>
public abstract class PageIndex : ComponentBase, IPageAction
{
    [Inject, NotNull] IPageLocatorService? PageLocator { get; set; }
    protected abstract Type? GetPageType(IPageLocatorService pageLocator);
    IPageAction? page;
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        var type = GetPageType(PageLocator);
        if (type != null)
        {
            builder.OpenComponent(0, type);
            builder.AddComponentReferenceCapture(1, obj =>
            {
                page = obj as IPageAction;
            });
            builder.CloseComponent();
        }
    }

    public Task OnShowAsync()
    {
        if (page != null)
        {
            return page.OnShowAsync();
        }
        return Task.CompletedTask;
    }

    public Task OnHiddenAsync()
    {
        if (page != null)
        {
            return page.OnHiddenAsync();
        }
        return Task.CompletedTask;
    }
}
