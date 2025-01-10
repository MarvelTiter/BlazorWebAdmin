using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Project.Constraints.Common.Attributes;
using Project.Constraints.PageHelper;
using System.Diagnostics.CodeAnalysis;

namespace Project.Web.Shared.Pages
{

    public abstract class SystemPageIndex : ComponentBase, IPageAction
    {
        [Inject, NotNull] IProjectSettingService? SettingProvider { get; set; }
        [Inject, NotNull] IPageLocatorService? Locator { get; set; }
        protected Type? PageType { get; set; }
        protected override void OnInitialized()
        {
            base.OnInitialized();
            PageType = GetPageType(Locator);
        }
        public abstract Type? GetPageType(IPageLocatorService customSetting);
        IPageAction? page;
        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            if (PageType != null)
            {
                builder.OpenComponent(0, PageType);
                builder.AddComponentReferenceCapture(1, obj =>
                {
                    page = obj as IPageAction;
                });
                builder.CloseComponent();
            }
        }

        public async Task OnShowAsync()
        {
            if (page != null)
                await page.OnShowAsync();
        }

        public async Task OnHiddenAsync()
        {
            if (page != null)
                await page.OnHiddenAsync();
        }
    }

    [Route("/user/index")]
    [PageGroup("BasicSetting", "BasicSetting", 2, Icon = "fa fa-cog")]
    [PageInfo(Title = "用户管理", Icon = "svg-user", Sort = 1, GroupId = "BasicSetting")]
    public class UserIndex : SystemPageIndex
    {
        public override Type? GetPageType(IPageLocatorService customSetting)
        {
            return customSetting.GetUserPageType();
        }
    }

    [Route("/operationlog")]
    [PageInfo(Title = "操作日志", Icon = "svg-log", Sort = 2, GroupId = "BasicSetting")]
    public class RunLogIndex : SystemPageIndex
    {
        public override Type? GetPageType(IPageLocatorService customSetting)
        {
            return customSetting.GetRunLogPageType();
        }
    }

    [Route("/rolepermission")]
    [PageGroup("SysSetting", "SysSetting", 2, Icon = "fa fa-cog")]
    [PageInfo(Id = "RolePermission", Title = "权限分配", Icon = "svg-assign_permissions", Sort = 1, GroupId = "SysSetting")]
    public class RolePermissionIndex : SystemPageIndex
    {
        public override Type? GetPageType(IPageLocatorService customSetting)
        {
            return customSetting.GetRolePermissionPageType();
        }
    }

    [Route("/permission")]
    [PageInfo(Id = "Permission", Title = "权限设置", Icon = "svg-rights", Sort = 2, GroupId = "SysSetting")]
    public class PermissionIndex : SystemPageIndex
    {
        public override Type? GetPageType(IPageLocatorService customSetting)
        {
            return customSetting.GetPermissionPageType();
        }
    }

}
