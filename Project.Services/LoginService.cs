using Project.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Services
{
    public class LoginService : ILoginService
    {
        public Task<bool> LoginAsync(string username, string password, out string token)
        {
            token = string.Empty;
            return Task.FromResult(true);
        }

        public Task<bool> LogoutAsync()
        {
            throw new NotImplementedException();
        }
    }
}
