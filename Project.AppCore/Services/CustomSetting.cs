using MDbContext.ExpressionSql;
using MDbContext.Repository;
using Project.AppCore.SystemPermission;
using Project.Models.Entities.Permissions;

namespace Project.Services
{
    public class CustomSetting : ICustomSettingProvider
    {
        private readonly IExpressionContext context;

        public CustomSetting(IExpressionContext context)
        {
            this.context = context;
        }

        public Type? GetDashboardType() => null;
        public Type GetUserPageType() => typeof(CustomUserPage);

        public async Task<IQueryResult<UserInfo>> GetUserInfoAsync(string username, string password)
        {
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
            //var userInfo = new UserInfo
            //{
            //    UserId = username,
            //    UserName = u.UserName,
            //    Roles = roles.Select(ur => ur.RoleId).ToList()
            //};
            userInfo.Roles = roles.Select(ur => ur.RoleId).ToList();

            return result;
        }


        public async Task<int> UpdateLoginInfo(UserInfo info)
        {
            return await context.Update<User>()
                                    .Set(u => u.LastLogin, DateTime.Now)
                                    .Where(u => u.UserId == info.UserId).ExecuteAsync();
        }
    }
}
