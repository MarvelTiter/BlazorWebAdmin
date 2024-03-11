using MDbContext.ExpressionSql;
using MDbContext.Repository;
using Microsoft.AspNetCore.Components;
using Project.AppCore;
using Project.Constraints.Models.Permissions;
using Project.Constraints.Store;
using Project.Constraints.Store.Models;
using Project.Web.Shared.Components;

namespace Project.Services
{
    public class CustomSetting : BasicCustomSetting, ICustomSettingService
    {
        private readonly IExpressionContext context;
        private readonly NavigationManager navigation;
        private readonly IUserStore userStore;
        private readonly IWatermarkServiceFactory watermarkServiceFactory;

        public CustomSetting(IExpressionContext context, NavigationManager navigation, IUserStore userStore, IWatermarkServiceFactory watermarkServiceFactory)
        {
            this.context = context;
            this.navigation = navigation;
            this.userStore = userStore;
            this.watermarkServiceFactory = watermarkServiceFactory;
        }

        public override async Task<IQueryResult<UserInfo>> GetUserInfoAsync(string username, string password)
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

            userInfo.Roles = roles.Select(ur => ur.RoleId).ToList();

            return result;
        }

        public override async Task<int> UpdateLoginInfo(UserInfo info)
        {
            return await context.Update<User>()
                                    .Set(u => u.LastLogin, DateTime.Now)
                                    .Where(u => u.UserId == info.UserId).ExecuteAsync();
        }

        //public override Task AfterWebApplicationAccessed()
        //{
        //    var service = watermarkServiceFactory.GetWatermarkService();
        //    service.UpdateWaterMarkAsync(userStore.UserInfo?.UserName!, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        //    return Task.CompletedTask;
        //}

        //public override Task RouterChangingAsync(TagRoute route)
        //{
        //    var service = watermarkServiceFactory.GetWatermarkService();
        //    service.UpdateWaterMarkAsync(userStore.UserInfo?.UserName!, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), route.RouteTitle);
        //    return Task.CompletedTask;
        //}
    }
}
