using Microsoft.AspNetCore.Authorization;
using Project.Constraints.Common.Attributes;

namespace Project.Web.Shared.Pages;
#if (ExcludeDefaultPages)
#else
[Route("/user/index")]
[PageGroup("BasicSetting", "基础配置", 1, Icon = "fa fa-cog")]
[PageInfo(Title = "用户管理", Icon = "svg-user", Sort = 1, GroupId = "BasicSetting")]
[Authorize]
public class UserIndex : SystemPageIndex<UserIndex>
{
    protected override Type? GetPageType(IPageLocatorService customSetting)
    {
        return customSetting.GetUserPageType();
    }
}

[Route("/operationlog")]
[PageInfo(Title = "操作日志", Icon = "svg-log", Sort = 2, GroupId = "BasicSetting")]
[Authorize]
public class RunLogIndex : SystemPageIndex<RunLogIndex>
{
    protected override Type? GetPageType(IPageLocatorService customSetting)
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
    protected override Type? GetPageType(IPageLocatorService customSetting)
    {
        return customSetting.GetRolePermissionPageType();
    }
}

[Route("/permission")]
[PageInfo(Id = "Permission", Title = "权限设置", Icon = "svg-rights", Sort = 2, GroupId = "SysSetting")]
[Authorize]
public class PermissionIndex : SystemPageIndex<PermissionIndex>
{
    protected override Type? GetPageType(IPageLocatorService customSetting)
    {
        return customSetting.GetPermissionPageType();
    }
}
#endif