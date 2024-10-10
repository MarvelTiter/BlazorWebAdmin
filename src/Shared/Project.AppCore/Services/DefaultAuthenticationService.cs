using AutoAopProxyGenerator;
using AutoInjectGenerator;
using LightORM;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Project.Constraints.Models.Permissions;
using Project.Constraints.Store;
using Project.Web.Shared.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        
        public virtual Task<QueryResult> CheckUserPasswordAsync(UserPwd pwd) => Task.FromResult(Result.Success());
        
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
            userInfo.Roles = [.. roles];
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
            await ctx.SignOutAsync();
            var redirect = ctx.Request.Query["Redirect"];
            ctx.Response.Redirect($"/account/login?Redirect={redirect}");
        }
    }
}