using MDbContext.ExpressionSql;
using MDbContext.Repository;
using Project.AppCore;
using Project.AppCore.SystemPermission;
using Project.Constraints.Models.Permissions;

namespace Project.Services
{
    [IgnoreAutoInject]
    public abstract class BasicCustomSetting : ICustomSettingProvider
    {
        static Type? UserPageType;
        static Type? PermissionPageType;
        static Type? RolePermissionPageType;
        static Type? RunLogPageType;
        public BasicCustomSetting(ProjectSetting setting)
        {
            UserPageType ??= typeof(UserPage<,,>).MakeGenericType(setting.UserType, setting.PowerType, setting.RoleType);
            PermissionPageType ??= typeof(PermissionSetting<,>).MakeGenericType(setting.PowerType, setting.RoleType);
            RolePermissionPageType ??= typeof(RolePermission<,>).MakeGenericType(setting.PowerType, setting.RoleType);
            RunLogPageType ??= typeof(OperationLog<>).MakeGenericType(setting.RunlogType);
        }
        public virtual Type? GetDashboardType() => null;
        public virtual Type? GetUserPageType() => UserPageType;
        public virtual Type? GetPermissionPageType() => PermissionPageType;
        public virtual Type? GetRolePermissionPageType() => RolePermissionPageType;
        public virtual Type? GetRunLogPageType() => RunLogPageType;


        public abstract Task<IQueryResult<UserInfo>> GetUserInfoAsync(string username, string password);
        public abstract Task<int> UpdateLoginInfo(UserInfo info);
    }
    public class CustomSetting : BasicCustomSetting, ICustomSettingProvider
    {
        private readonly IExpressionContext context;

        public CustomSetting(IExpressionContext context, ProjectSetting setting) : base(setting)
        {
            this.context = context;
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
            //var userInfo = new UserInfo
            //{
            //    UserId = username,
            //    UserName = u.UserName,
            //    Roles = roles.Select(ur => ur.RoleId).ToList()
            //};
            userInfo.Roles = roles.Select(ur => ur.RoleId).ToList();

            return result;
        }


        public override async Task<int> UpdateLoginInfo(UserInfo info)
        {
            return await context.Update<User>()
                                    .Set(u => u.LastLogin, DateTime.Now)
                                    .Where(u => u.UserId == info.UserId).ExecuteAsync();
        }
    }
}
