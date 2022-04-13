using Project.Models;
using Project.Services.interfaces;

namespace Project.Services
{
    public class LoginService : ILoginService
    {
        public Task<QueryResult<string>> LoginAsync(string username, string password)
        {
            return Task.FromResult(new QueryResult<string>() { Success = true, Message = "Done" });
        }

        public Task<bool> LogoutAsync()
        {
            throw new NotImplementedException();
        }
    }
}
