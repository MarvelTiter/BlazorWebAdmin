using Microsoft.AspNetCore.Mvc;
using Project.AppCore.Auth;
using System.Text;
using System.Text.Json;
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

    //public class LoginServiceApiInvoker : ILoginService
    //{
    //    private readonly IHttpClientFactory clientFactory;
    //    private readonly JsonSerializerOptions jsonOptions = new() { PropertyNameCaseInsensitive = true };
    //    public LoginServiceApiInvoker(IHttpClientFactory factory)
    //    {
    //        clientFactory = factory;
    //    }

    //    public async Task<bool> CheckUser(UserInfo info)
    //    {
    //        var client = clientFactory.CreateClient(nameof(LoginServiceApiInvoker));
    //        var request = new HttpRequestMessage();
    //        var jsonPayload = System.Text.Json.JsonSerializer.Serialize(info);
    //        request.Content = new StringContent(jsonPayload, Encoding.Default, "application/json");
    //        request.Method = HttpMethod.Post;
    //        request.RequestUri = new Uri("/api/LoginService/CheckUser", UriKind.Relative);
    //        var response = await client.SendAsync(request);
    //        response.EnsureSuccessStatusCode();
    //        var jsonStream = await response.Content.ReadAsStreamAsync();
    //        return System.Text.Json.JsonSerializer.Deserialize<bool>(jsonStream, jsonOptions);
    //    }

    //    public Task<IQueryResult<UserInfo>> LoginAsync(string username, string password)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public Task<IQueryResult<bool>> LogoutAsync()
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public Task<IQueryResult<bool>> UpdateLastLoginTimeAsync(UserInfo info)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}
}
