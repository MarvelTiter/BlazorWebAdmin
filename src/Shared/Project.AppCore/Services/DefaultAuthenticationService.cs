﻿using AutoAopProxyGenerator;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Project.Constraints.Models.Permissions;

namespace Project.AppCore.Services
{
    [GenAspectProxy]
    public class DefaultAuthenticationService : IAuthService
    {
        //private readonly IExpressionContext context;
        //private readonly IHttpContextAccessor httpContextAccessor;
        protected IServiceProvider Services { get; }

        public DefaultAuthenticationService(IServiceProvider services)
        {
            Services = services;
        }
#if (ExcludeDefaultService)
        public virtual Task<QueryResult<UserInfo>> SignInAsync(LoginFormModel loginForm) => throw new NotImplementedException();
        
        public virtual Task<QueryResult> CheckUserPasswordAsync(UserPwd pwd) => Task.FromResult(QueryResult.Success());
        
        public virtual Task<QueryResult> ModifyUserPasswordAsync(UserPwd pwd) => throw new NotImplementedException();
#else
        public virtual async Task<QueryResult<UserInfo>> SignInAsync(LoginFormModel loginForm)
        {
            var context = Services.GetService<IExpressionContext>();
            ArgumentNullException.ThrowIfNull(context);
            var username = loginForm.UserName;
            var password = loginForm.Password;
            var u = await context.Select<User>().Where(u => u.UserId == username).FirstAsync();
            var userInfo = new UserInfo
            {
                UserId = username,
                UserName = u?.UserName ?? "",
            };

            var result = userInfo.Result(u != null);
            if (!result.IsSuccess)
            {
                result.Message = $"用户：{username} 不存在";
                return result;
            }

            if (u!.Password != password)
            {
                result.Message = "密码错误";
                result.IsSuccess = false;
                return result;
            }

            var roles = await context.Select<UserRole>().Where(ur => ur.UserId == username).ToListAsync(r => r.RoleId);
            var projSetting = Services.GetService<IProjectSettingService>();
            var powers = await projSetting!.GetUserPowersAsync(userInfo);
            userInfo.Roles = [.. roles];
            userInfo.UserPowers = [.. powers.Where(p => p.PowerType != PowerType.Page).Select(p => p.PowerId)];
            userInfo.UserPages = [.. powers.Where(p => p.PowerType == PowerType.Page).Select(p => p.PowerId)];
            return result;
        }

        public virtual async Task<QueryResult> CheckUserPasswordAsync(UserPwd pwd)
        {
            var context = Services.GetService<IExpressionContext>();
            ArgumentNullException.ThrowIfNull(context);
            var old = await context.Select<User>()
                .Where(u => u.UserId == pwd.UserId)
                .FirstAsync();
            return old?.Password == pwd.OldPassword;
        }

        public virtual async Task<QueryResult> ModifyUserPasswordAsync(UserPwd pwd)
        {
            var context = Services.GetService<IExpressionContext>();
            ArgumentNullException.ThrowIfNull(context);
            var r = await context.Update<User>().Set(u => u.Password, pwd.Password)
                .Where(u => u.UserId == pwd.UserId)
                .ExecuteAsync();
            return r > 0;
        }
#endif
        public virtual async Task SignOutAsync()
        {
            var httpContextAccessor = Services.GetService<IHttpContextAccessor>();
            ArgumentNullException.ThrowIfNull(httpContextAccessor);
            var ctx = httpContextAccessor.HttpContext;
            if (ctx == null)
            {
                return;
            }

            // var ctx = httpContextAccessor.HttpContext.User;
            await ctx.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            // 解码了
            var redirect = ctx.Request.Query["Redirect"];
            // 重新编码
            var encoded = Uri.EscapeDataString(redirect!);// System.Web.HttpUtility.HtmlEncode(redirect);
            ctx.Response.Redirect($"/account/login?Redirect={encoded}");
        }
    }
}