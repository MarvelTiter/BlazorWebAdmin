using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Project.Constraints.Common.Attributes;
using Project.Constraints.PageHelper;
using System.Diagnostics.CodeAnalysis;

namespace Project.Web.Shared.Pages;

public abstract class SystemPageIndex<TPage> : AppComponentBase, IRoutePage
    where TPage : SystemPageIndex<TPage>
{
    //[Inject, NotNull] IProjectSettingService? SettingProvider { get; set; }
    [Inject, NotNull] IPageLocatorService? Locator { get; set; }
    protected Type? PageType { get; set; }
    protected virtual bool CascadingSelf => true;
    protected virtual bool CascadingFixed => true;
    protected override void OnInitialized()
    {
        base.OnInitialized();
        PageType = GetPageType(Locator);
    }
    public abstract Type? GetPageType(IPageLocatorService customSetting);
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
                    child.AddComponentReferenceCapture(1, obj =>
                    {
                        page = obj as IRoutePage;
                    });
                    child.CloseComponent();
                }));
                builder.CloseComponent();
            }
            else
            {
                builder.OpenComponent(0, PageType);
                builder.AddComponentReferenceCapture(1, obj =>
                {
                    page = obj as IRoutePage;
                });
                builder.CloseComponent();
            }
        }
    }

    public void OnClose() => page?.OnClose();
}
#if (ExcludeDefaultPages)
#else
[Route("/user/index")]
[PageGroup("BasicSetting", "基础配置", 1, Icon = "fa fa-cog")]
[PageInfo(Title = "用户管理", Icon = "svg-user", Sort = 1, GroupId = "BasicSetting")]
[Authorize]
public class UserIndex : SystemPageIndex<UserIndex>
{
    protected override bool CascadingSelf => false;
    public override Type? GetPageType(IPageLocatorService customSetting)
    {
        return customSetting.GetUserPageType();
    }
}

[Route("/operationlog")]
[PageInfo(Title = "操作日志", Icon = "svg-log", Sort = 2, GroupId = "BasicSetting")]
[Authorize]
public class RunLogIndex : SystemPageIndex<RunLogIndex>
{
    protected override bool CascadingSelf => false;
    public override Type? GetPageType(IPageLocatorService customSetting)
    {
        return customSetting.GetRunLogPageType();
    }
}

[Route("/rolepermission")]
[PageGroup("SysSetting", "系统设置", 2, Icon = "fa fa-cog")]
[PageInfo(Id = "RolePermission", Title = "权限分配", Icon = "svg-assign_permissions", Sort = 1, GroupId = "SysSetting")]
[Authorize]
public class RolePermissionIndex : SystemPageIndex<RolePermissionIndex>
{
    protected override bool CascadingSelf => false;
    public override Type? GetPageType(IPageLocatorService customSetting)
    {
        return customSetting.GetRolePermissionPageType();
    }
}

[Route("/permission")]
[PageInfo(Id = "Permission", Title = "权限设置", Icon = "svg-rights", Sort = 2, GroupId = "SysSetting")]
[Authorize]
public class PermissionIndex : SystemPageIndex<PermissionIndex>
{
    protected override bool CascadingSelf => false;
    public override Type? GetPageType(IPageLocatorService customSetting)
    {
        return customSetting.GetPermissionPageType();
    }
}
#endif