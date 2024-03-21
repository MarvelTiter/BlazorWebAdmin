using LightORM;
using Project.Constraints.Models.Permissions;

namespace Project.AppCore
{

    public class DbInitializer : DbInitialContext
    {
        List<Power> powers =
        [
            new()
            {
                PowerId = "ROOT",
                PowerName = "/",
                ParentId = "",
                PowerType = PowerType.Page,
                Icon = "",
                Path = "",
                Sort = 1,
                PowerLevel = 0
            },
            new()
            {
                PowerId = "BasicSetting",
                PowerName = "基础配置",
                ParentId = "ROOT",
                PowerType = PowerType.Page,
                Icon = "setting",
                Path = "",
                Sort = 1,
                PowerLevel = 1
            },
            new()
            {
                PowerId = "Setting",
                PowerName = "系统设置",
                ParentId = "ROOT",
                PowerType = PowerType.Page,
                Icon = "setting",
                Path = "",
                Sort = 2,
                PowerLevel = 1
            },
            new()
            {
                PowerId = "User",
                PowerName = "用户",
                ParentId = "BasicSetting",
                PowerType = PowerType.Page,
                Icon = "user",
                Path = "user/index",
                Sort = 1,
                PowerLevel = 2
            },new()
            {
                PowerId = "OperationLog",
                PowerName = "操作日志",
                ParentId = "BasicSetting",
                PowerType = PowerType.Page,
                Icon = "log",
                Path = "operationlog",
                Sort = 2,
                PowerLevel = 2
            },new()
            {
                PowerId = "RolePermission",
                PowerName = "权限分配",
                ParentId = "Setting",
                PowerType = PowerType.Page,
                Icon = "assign_permissions",
                Path = "rolepermission",
                Sort = 1,
                PowerLevel = 2
            },new()
            {
                PowerId = "Permission",
                PowerName = "权限设置",
                ParentId = "Setting",
                PowerType = PowerType.Page,
                Icon = "rights",
                Path = "permission",
                Sort = 2,
                PowerLevel = 2
            }
        ];
        public override void Initialized(IDbInitial db)
        {
            db.CreateTable<Power>(datas:[.. powers]);
        }
    }
}
