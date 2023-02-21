using MDbContext.ExpressionSql;
using MDbContext.Repository;
using Project.AppCore.Auth;
using Project.AppCore.Services;
using Project.Models;
using Project.Models.Entities;
using Project.Models.Permissions;

namespace Project.Services
{
    public partial class LoginService : ILoginService
    {
        private readonly IExpressionContext context;

        public LoginService(IExpressionContext context)
        {
            this.context = context;
        }
        public Task<bool> CheckUser(UserInfo info)
        {
            return Task.FromResult(true);
        }

        public async Task<IQueryResult<UserInfo>> LoginAsync(string username, string password)
        {
            var u = await context.Repository<User>().GetSingleAsync(u => u.UserId == username);
            var userInfo = new UserInfo
            {
                UserId = username,
                UserName = u?.UserName ?? "",
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
            //var userInfo = new UserInfo
            //{
            //    UserId = username,
            //    UserName = u.UserName,
            //    Roles = roles.Select(ur => ur.RoleId).ToList()
            //};
            userInfo.Roles = roles.Select(ur => ur.RoleId).ToList();
            await UpdateLastLoginTimeAsync(userInfo);
            //result.SetPayload(userInfo);
            return result;
        }

        public async Task<IQueryResult<bool>> UpdateLastLoginTimeAsync(UserInfo info)
        {
            var flag = await context.Update<User>()
                                    .Set(u => u.LastLogin, DateTime.Now)
                                    .Where(u => u.UserId == info.UserId).ExecuteAsync();
            info.Payload = JwtTokenHelper.GetToken(info.UserId, null, info.Roles.ToArray());
            return (flag > 0).Result();
        }

        public Task<IQueryResult<bool>> LogoutAsync()
        {
            //TODO 用户登出处理
            return Task.FromResult((true).Result());
		}
    }
}
