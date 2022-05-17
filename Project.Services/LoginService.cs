using Project.Models;
using Project.Services.interfaces;

namespace Project.Services
{
    public class UserInfo
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public object? Payload { get; set; }
        public IEnumerable<string> Roles { get; set; }
        public DateTime CreatedTime { get; set; }
        public UserInfo()
        {
            UserId = string.Empty;
            Payload = null;
            Roles = Enumerable.Empty<string>();
            CreatedTime = DateTime.Now;
        }
    }
    public class LoginService : ILoginService
    {
        public Task<bool> CheckUser(UserInfo info)
        {
            return Task.FromResult(true);
        }

        public Task<QueryResult<UserInfo>> LoginAsync(string username, string password)
        {
            var roles = new List<string> { "admin", "superadmin", "vistor" };
            return Task.FromResult(new QueryResult<UserInfo>() { Success = true, Message = "Done", Payload = new UserInfo { UserId = username , Roles= roles } });
        }

        public Task<bool> LogoutAsync()
        {
            throw new NotImplementedException();
        }
    }
}
