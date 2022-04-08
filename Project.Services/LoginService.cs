using Project.IServices;
using Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
