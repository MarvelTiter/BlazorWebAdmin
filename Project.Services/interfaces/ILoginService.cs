using Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Services.interfaces
{
    public interface ILoginService
    {
        Task<QueryResult<UserInfo>> LoginAsync(string username, string password);
        Task<bool> LogoutAsync();
    }
}
