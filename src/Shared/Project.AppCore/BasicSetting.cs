using Project.Constraints;
using Project.Constraints.Models.Permissions;
using Project.Constraints.Store.Models;
using Project.Web.Shared.Pages;

namespace Project.AppCore
{
    [IgnoreAutoInject]
    public abstract class BasicSetting : IProjectSettingService//, IDisposable
    {
        static Type? UserPageType;
        static Type? PermissionPageType;
        static Type? RolePermissionPageType;
        static Type? RunLogPageType;
        protected UserInfo? CurrentUser { get; set; }
        public BasicSetting()
        {
            UserPageType ??= typeof(UserPage<,,>).MakeGenericType(AppConst.TypeInfo.UserType, AppConst.TypeInfo.PowerType, AppConst.TypeInfo.RoleType);
            PermissionPageType ??= typeof(PermissionSetting<,>).MakeGenericType(AppConst.TypeInfo.PowerType, AppConst.TypeInfo.RoleType);
            RolePermissionPageType ??= typeof(RolePermission<,>).MakeGenericType(AppConst.TypeInfo.PowerType, AppConst.TypeInfo.RoleType);
            RunLogPageType ??= typeof(OperationLog<>).MakeGenericType(AppConst.TypeInfo.RunlogType);
        }

        public virtual Type? GetDashboardType() => null;
        public virtual Type? GetUserPageType() => UserPageType;
        public virtual Type? GetPermissionPageType() => PermissionPageType;
        public virtual Type? GetRolePermissionPageType() => RolePermissionPageType;
        public virtual Type? GetRunLogPageType() => RunLogPageType;

        public abstract Task<IQueryResult<UserInfo>> GetUserInfoAsync(string username, string password);
        public abstract Task<int> UpdateLoginInfo(UserInfo info);
        public virtual Task LoginSuccessAsync(UserInfo result)
        {
            CurrentUser = result;
            return Task.CompletedTask;
        }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public virtual Task<bool> LoginInterceptorAsync(UserInfo result)
        {
            return Task.FromResult(true);
        }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public virtual Task AfterWebApplicationAccessed()
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public virtual Task<IEnumerable<IPower>> GetUserPowersAsync(UserInfo info) => Task.FromResult<IEnumerable<IPower>>([]);

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="route"></param>
        /// <returns></returns>
        public virtual Task<bool> RouterChangingAsync(TagRoute route)
        {
            return Task.FromResult(true);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="meta"></param>
        /// <returns></returns>
        public virtual Task<bool> RouteMetaFilterAsync(RouterMeta meta)
        {
            return Task.FromResult(true);
        }
    }
}
