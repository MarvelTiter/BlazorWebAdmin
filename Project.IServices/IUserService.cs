using Project.Models;
using Project.Models.Entities;
using Project.Models.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.IServices
{
    public interface IUserService
    {
        Task<User> LoginAsync(LoginFormModel loginForm);
        Task<IEnumerable<string>> GetUserRolesAsync(string userId);
        Task<IEnumerable<RouterMeta>> GetUserPermissionAsync(string userId);
    }
}
