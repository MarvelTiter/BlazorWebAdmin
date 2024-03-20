using Project.AppCore.SystemPermission;
using Project.Constraints.Store.Models;

namespace Project.AppCore
{
    [IgnoreAutoInject]
    public abstract class BasicCustomSetting : ICustomSettingService
    {
        static Type? UserPageType;
        static Type? PermissionPageType;
        static Type? RolePermissionPageType;
        static Type? RunLogPageType;

        List<IAddtionalTnterceptor> initActions = [];

        public event Func<IQueryResult<UserInfo>, Task> OnLoginSuccessAsync;

        public event Func<TagRoute, Task> OnRouterChangingAsync;
        public BasicCustomSetting()
        {
            UserPageType ??= typeof(UserPage<,,>).MakeGenericType(Config.TypeInfo.UserType, Config.TypeInfo.PowerType, Config.TypeInfo.RoleType);
            PermissionPageType ??= typeof(PermissionSetting<,>).MakeGenericType(Config.TypeInfo.PowerType, Config.TypeInfo.RoleType);
            RolePermissionPageType ??= typeof(RolePermission<,>).MakeGenericType(Config.TypeInfo.PowerType, Config.TypeInfo.RoleType);
            RunLogPageType ??= typeof(OperationLog<>).MakeGenericType(Config.TypeInfo.RunlogType);
        }
        
        public void AddService(IAddtionalTnterceptor additional)
        {
            initActions.Add(additional);
            OnLoginSuccessAsync += additional.LoginSuccessAsync;
            OnRouterChangingAsync += additional.RouterChangingAsync;
        }

        public virtual Type? GetDashboardType() => null;
        public virtual Type? GetUserPageType() => UserPageType;
        public virtual Type? GetPermissionPageType() => PermissionPageType;
        public virtual Type? GetRolePermissionPageType() => RolePermissionPageType;
        public virtual Type? GetRunLogPageType() => RunLogPageType;


        public abstract Task<IQueryResult<UserInfo>> GetUserInfoAsync(string username, string password);
        public abstract Task<int> UpdateLoginInfo(UserInfo info);
        public virtual Task<bool> LoginSuccessAsync(IQueryResult<UserInfo> result)
        {
            OnLoginSuccessAsync?.Invoke(result);
            return Task.FromResult(true);
        }

        public virtual Task AfterWebApplicationAccessed()
        {
            return Task.CompletedTask;
        }

        public virtual Task<bool> RouterChangingAsync(TagRoute route)
        {
            OnRouterChangingAsync?.Invoke(route);
            return Task.FromResult(true);
        }

        public virtual Task<bool> RouteMetaFilterAsync(RouterMeta meta)
        {
            return Task.FromResult(true);
        }
    }
}
