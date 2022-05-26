using Project.AppCore.Services;
using Project.Models;
using Project.Models.Permissions;

namespace Project.Services
{
    public partial class LoginService : ILoginService
    {
        public Task<bool> CheckUser(UserInfo info)
        {
            return Task.FromResult(true);
        }

        public async Task<IQueryResult<UserInfo>> LoginAsync(string username, string password)
        {
            var roles = new List<string> { "admin", "superadmin", "vistor" };
            return await Task.FromResult(new QueryResult<UserInfo>() { Success = true, Message = "Done", Payload = new UserInfo { UserId = username, Roles = roles, UserName = "测试" } });
        }

        public Task<bool> LogoutAsync()
        {            
            throw new NotImplementedException();
        }
    }
}
