using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.IServices
{
    public interface ILoginService
    {
        Task<bool> LoginAsync(string username, string password, out string token);
        Task<bool> LogoutAsync();
    }
}
