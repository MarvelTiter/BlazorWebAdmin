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
    //[AutoInject]
    public class DefaultAuthenticationService : IAuthService
    {
        private readonly IExpressionContext context;
        private readonly IHttpContextAccessor httpContextAccessor;

        public DefaultAuthenticationService(IExpressionContext context, IHttpContextAccessor httpContextAccessor)
        {
            this.context = context;
            this.httpContextAccessor = httpContextAccessor;
        }
        public virtual async Task<QueryResult<UserInfo>> SignInAsync(LoginFormModel loginForm)
        {
            var username = loginForm.UserName;
            var password = loginForm.Password;
            var u = await context.Repository<User>().GetSingleAsync(u => u.UserId == username);
            var userInfo = new UserInfo
            {
                UserId = username,
                UserName = u?.UserName ?? "",
                Password = password
            };
            var result = userInfo.Result(u != null);
            if (!result.Success)
            {
                result.Message = $"用户：{username} 不存在";
                return result;
            }
            if (u!.Password != password)
            {
                result.Message = "密码错误";
                result.Success = false;
                return result;
            }
            var roles = await context.Repository<UserRole>().GetListAsync(ur => ur.UserId == username);
            userInfo.Roles = roles.Select(ur => ur.RoleId).ToArray();
            return result;
        }

        public virtual async Task SignOutAsync()
        {
            var ctx = httpContextAccessor.HttpContext;
            if (ctx == null)
            {
                return;
            }
            // var ctx = httpContextAccessor.HttpContext.User;
            await httpContextAccessor.HttpContext.SignOutAsync();
            var redirect = ctx.Request.Query["Redirect"];
            httpContextAccessor.HttpContext.Response.Redirect($"/account/login?Redirect={redirect}");
        }
    }
}
