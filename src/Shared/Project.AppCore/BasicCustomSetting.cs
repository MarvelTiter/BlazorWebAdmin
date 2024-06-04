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
        //readonly List<IAddtionalInterceptor> initActions = [];
        //protected readonly IAppSession appSession;
        protected UserInfo? CurrentUser { get; set; }
        public BasicSetting()
        {
            UserPageType ??= typeof(UserPage<,,>).MakeGenericType(Config.TypeInfo.UserType, Config.TypeInfo.PowerType, Config.TypeInfo.RoleType);
            PermissionPageType ??= typeof(PermissionSetting<,>).MakeGenericType(Config.TypeInfo.PowerType, Config.TypeInfo.RoleType);
            RolePermissionPageType ??= typeof(RolePermission<,>).MakeGenericType(Config.TypeInfo.PowerType, Config.TypeInfo.RoleType);
            RunLogPageType ??= typeof(OperationLog<>).MakeGenericType(Config.TypeInfo.RunlogType);
            //this.appSession = appSession;
            //this.appSession.RouterStore.RouterChangingEvent += RouterChangingAsync;
            //this.appSession.RouterStore.RouteMetaFilterEvent += RouteMetaFilterAsync;
            //this.appSession.UserStore.LoginSuccessEvent += LoginSuccessAsync;
            //this.appSession.WebApplicationAccessedEvent += AfterWebApplicationAccessed;
        }

        //public void AddService(IAddtionalInterceptor additional)
        //{
        //    initActions.Add(additional);
        //    appSession.RouterStore.RouterChangingEvent += additional.RouterChangingAsync;
        //    appSession.RouterStore.RouteMetaFilterEvent += additional.RouteMetaFilterAsync;
        //    appSession.UserStore.LoginSuccessEvent += additional.LoginSuccessAsync;
        //    appSession.WebApplicationAccessedEvent += additional.AfterWebApplicationAccessedAsync;
        //}

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

        //#region Dispose
        //private bool disposedValue;

        //protected virtual void Dispose(bool disposing)
        //{
        //    if (!disposedValue)
        //    {
        //        if (disposing)
        //        {
        //            //appSession.RouterStore.RouterChangingEvent -= RouterChangingAsync;
        //            //appSession.RouterStore.RouteMetaFilterEvent -= RouteMetaFilterAsync;
        //            //appSession.UserStore.LoginSuccessEvent -= LoginSuccessAsync;
        //            //appSession.WebApplicationAccessedEvent -= AfterWebApplicationAccessed;

        //            //foreach (var additional in initActions)
        //            //{
        //            //    appSession.UserStore.LoginSuccessEvent -= additional.LoginSuccessAsync;
        //            //    appSession.RouterStore.RouterChangingEvent -= additional.RouterChangingAsync;
        //            //    appSession.WebApplicationAccessedEvent -= additional.AfterWebApplicationAccessedAsync;
        //            //}
        //        }
        //        disposedValue = true;
        //    }
        //}


        //public void Dispose()
        //{
        //    Dispose(disposing: true);
        //    GC.SuppressFinalize(this);
        //}
        //#endregion
    }
}
