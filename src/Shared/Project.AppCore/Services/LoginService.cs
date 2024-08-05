using Project.AppCore.Auth;
using AutoWasmApiGenerator;
using AutoInjectGenerator;
using MT.Generators.Abstraction;
namespace Project.Services
{
    
    [AutoInject(Group = "SERVER")]
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
        [WebMethod(Method = WebMethod.Get)]
        public async Task<QueryResult<UserInfo>> LoginAsync(string username, string password)
        {
            var result = await settingProvider.GetUserInfoAsync(username, password);
            await UpdateLastLoginTimeAsync(result.Payload!);
            return result;
        }

        public async Task<QueryResult<bool>> UpdateLastLoginTimeAsync(UserInfo info)
        {
            var flag = await settingProvider.UpdateLoginInfo(info);
            info.ApiToken = JwtTokenHelper.GetToken(info.UserId, null, info.Roles.ToArray());
            return (flag > 0).Result();
        }

        public Task<QueryResult<bool>> LogoutAsync()
        {
            //TODO 用户登出处理
            return Task.FromResult((true).Result());
        }
    }
}
