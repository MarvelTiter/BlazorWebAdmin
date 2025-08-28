using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Project.Constraints.PageHelper;
using Project.Constraints.Services;
using Project.Constraints.Store;
using Project.Constraints.UI.Extensions;

namespace Project.Constraints.Page;

/// <summary>
/// <para>自定义页面</para>
/// <para>
/// 使用场景: 当路由地址确定，但是页面内容不确定时，可通过<see cref="IPageLocatorService"/>设置页面组件
/// </para>
/// <para>
/// 子类可重写<see cref="CascadingSelf"/>决定是否将自己作为级联值传递给实际渲染的页面组件, 默认为false
/// </para>
/// <para>
/// 比如登录页面为Login组件, 其路由地址为/account/login, 登录逻辑是写在Login组件中, 但是UI实际是通过其他组件渲染的
/// <para>此时可以选择将登录组件作为级联值传递给实际渲染的组件, 这样就可以在实际渲染的组件中通过级联参数获取到登录组件的实例, 从而调用登录组件中的方法和属性</para>
/// </para>
/// </summary>
/// <typeparam name="TPage"></typeparam>
public abstract class SystemPageIndex<TPage> : AppComponentBase, IRoutePage
    where TPage : SystemPageIndex<TPage>
{
    [Inject, NotNull] protected IPageLocatorService? Locator { get; set; }
    [Inject, NotNull] protected IRouterStore? RouterStore { get; set; }
    protected Type? PageType { get; set; }
    protected virtual bool CascadingSelf => false;
    protected virtual bool CascadingFixed => true;
    protected override void OnInitialized()
    {
        base.OnInitialized();
        PageType = GetPageType(Locator);
    }
    protected abstract Type? GetPageType(IPageLocatorService customSetting);
    IRoutePage? page;
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (PageType != null)
        {
            if (CascadingSelf)
            {
                builder.OpenComponent<CascadingValue<TPage>>(0);
                builder.AddAttribute(1, nameof(CascadingValue<TPage>.Name), typeof(TPage).Name);
                builder.AddAttribute(2, nameof(CascadingValue<TPage>.Value), this);
                builder.AddAttribute(3, nameof(CascadingValue<TPage>.IsFixed), CascadingFixed);
                builder.AddAttribute(4, nameof(CascadingValue<TPage>.ChildContent), (RenderFragment)(child =>
                {
                    child.OpenComponent(0, PageType);
                    child.AddComponentReferenceCapture(1, HandleReferenceCapture);
                    child.CloseComponent();
                }));
                builder.CloseComponent();
            }
            else
            {
                builder.OpenComponent(0, PageType);
                builder.AddComponentReferenceCapture(1, HandleReferenceCapture);
                builder.CloseComponent();
            }
        }
    }

    public void OnClose() => page?.OnClose();

    private void HandleReferenceCapture(object ins)
    {
        if (RouterStore.Current is not null && ins is IRoutePage page)
        {
            this.page = page;
            var title = page.GetTitle();
            if (string.IsNullOrEmpty(title)) return;
            RouterStore.Current.Title = title.AsContent();
            if (RouterStore.Current.Menu is not null)
                RouterStore.Current.Menu.RouteTitle = title;
        }
    }

}
