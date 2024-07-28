﻿using Project.Constraints;
using Project.Constraints.Models.Permissions;
using Project.Constraints.Store.Models;
using Project.Web.Shared.Pages;

namespace Project.AppCore
{
    public abstract class BasicSetting : IProjectSettingService//, IDisposable
    {
        protected UserInfo? CurrentUser { get; set; }

        public abstract Task<QueryResult<UserInfo>> GetUserInfoAsync(string username, string password);
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
