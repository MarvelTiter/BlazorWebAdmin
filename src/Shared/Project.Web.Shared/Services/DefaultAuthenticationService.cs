using AutoInjectGenerator;
using LightORM;
using Project.Constraints.Models.Permissions;
using Project.Web.Shared.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Web.Shared.Services
{
    [AutoInject]
    public class DefaultAuthenticationService : IAuthenticationService
    {
        private readonly IExpressionContext context;

        public DefaultAuthenticationService(IExpressionContext context)
        {
            this.context = context;
        }
        public async Task<QueryResult<UserInfo>> SignInAsync(LoginFormModel loginForm)
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

            userInfo.Roles = roles.Select(ur => ur.RoleId).ToList();
            return result;
        }

        public Task<QueryResult> SignOutAsync(string? token)
        {
            return Task.FromResult(Result.Success());
        }
    }
}
