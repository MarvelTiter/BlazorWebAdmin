using Project.Models;
using Project.Services.interfaces;

namespace Project.Services
{
    public class UserInfo
    {
        public string UserName { get; set; }
        public object? Payload { get; set; }
        public IEnumerable<string> Roles { get; set; }
        public UserInfo()
        {
            UserName = string.Empty;
            Payload = null;
            Roles = Enumerable.Empty<string>();
        }
    }
    public class LoginService : ILoginService
    {

        public Task<QueryResult<UserInfo>> LoginAsync(string username, string password)
        {
            return Task.FromResult(new QueryResult<UserInfo>() { Success = true, Message = "Done", Payload = new UserInfo { UserName = username } });
        }

        public Task<bool> LogoutAsync()
        {
            throw new NotImplementedException();
        }
    }
}
