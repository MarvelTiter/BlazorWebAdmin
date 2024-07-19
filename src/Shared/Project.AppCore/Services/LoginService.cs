﻿using Project.AppCore.Auth;
namespace Project.Services
{
    [WebApiGenerator.Attributes.WebController]
    public partial class LoginService : ILoginService
    {
        private readonly IProjectSettingService settingProvider;

        public LoginService(IProjectSettingService settingProvider)
        {
            this.settingProvider = settingProvider;
        }

        public Task<bool> CheckUser(UserInfo info)
        {
            return Task.FromResult(true);
        }

        public async Task<IQueryResult<UserInfo>> LoginAsync(string username, string password)
        {
            var result = await settingProvider.GetUserInfoAsync(username, password);
            await UpdateLastLoginTimeAsync(result.Payload!);
            return result;
        }

        public async Task<IQueryResult<bool>> UpdateLastLoginTimeAsync(UserInfo info)
        {
            var flag = await settingProvider.UpdateLoginInfo(info);
            info.ApiToken = JwtTokenHelper.GetToken(info.UserId, null, info.Roles.ToArray());
            return (flag > 0).Result();
        }

        public Task<IQueryResult<bool>> LogoutAsync()
        {
            //TODO 用户登出处理
            return Task.FromResult((true).Result());
        }
    }
}
