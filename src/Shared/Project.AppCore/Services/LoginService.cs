using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project.AppCore.Auth;
using Project.Constraints.Store;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Project.Services
{
    [ApiController]
    [Route("[controller]/[action]")]
    [Authorize]
    public class LoginController
    {
        private readonly LoginService service;

        public LoginController(LoginService service)
        {
            this.service = service;
        }

        [HttpGet]
        public Task<IQueryResult<UserInfo>> Login(string username, string password) => service.LoginAsync(username, password);

    }

    public class LoginServiceClient : ILoginService
    {
        private readonly IHttpClientFactory factory;
        private readonly IUserStore store;

        public LoginServiceClient(IHttpClientFactory factory, IUserStore store)
        {
            this.factory = factory;
            this.store = store;
        }
        public Task<bool> CheckUser(UserInfo info)
        {
            throw new NotImplementedException();
        }

        public async Task<IQueryResult<UserInfo>> LoginAsync(string username, string password)
        {
            var client = factory.CreateClient("LoginServiceClient");
            var result = await client.SendAsync(new HttpRequestMessage());
            var content = await result.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<IQueryResult<UserInfo>>(content);
        }

        public Task<IQueryResult<bool>> LogoutAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IQueryResult<bool>> UpdateLastLoginTimeAsync(UserInfo info)
        {
            throw new NotImplementedException();
        }
    }
        
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
