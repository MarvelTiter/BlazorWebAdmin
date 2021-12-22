using BlazorWebAdmin.Models;
using BlazorWebAdmin.Models.Entities;
using BlazorWebAdmin.Models.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorWebAdmin.IServices
{
    public interface IUserService
    {
        User Login(LoginFormModel loginForm);
        IEnumerable<string> GetUserRoles(string userId);
        IEnumerable<RouterMeta> GetUserPermission(string userId);
    }
}
