using BlazorWebAdmin.IRepositories;
using BlazorWebAdmin.IServices;
using BlazorWebAdmin.Models;
using BlazorWebAdmin.Models.Entities;
using BlazorWebAdmin.Models.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorWebAdmin.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository userRepository;

        public UserService(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        public IEnumerable<RouterMeta> GetUserPermission(string userId)
        {
            var meta = new RouterMeta
            {
                RouteName = "设置",
                IconName = "setting",
                Children = new List<RouterMeta>
                    {
                        new RouterMeta
                        {
                            RouteName = "计数器",
                            IconName = "setting",
                            RouteLink = "counter",
                        },
                        new RouterMeta
                        {
                            RouteName = "表格",
                            IconName = "setting",
                            RouteLink = "table",
                        }
                    }
            };
            List<RouterMeta> result = new List<RouterMeta>();
            result.Add(meta);
            return result; 
        }

        public IEnumerable<string> GetUserRoles(string userId)
        {
            throw new NotImplementedException();
        }

        public User Login(LoginFormModel loginForm)
        {
            if (loginForm.UserName == "admin" && loginForm.Password == "admin")
            {
                return userRepository.GetSingle(null);
            }
            return null; 
        }
    }
}
