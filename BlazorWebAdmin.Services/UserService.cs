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

        public Task<IEnumerable<RouterMeta>> GetUserPermissionAsync(string userId)
        {
            IEnumerable<RouterMeta> result = new List<RouterMeta>
            {
                new RouterMeta
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
                }
            };
            return Task.FromResult(result);
        }

        public Task<IEnumerable<string>> GetUserRolesAsync(string userId)
        {
            throw new NotImplementedException();
        }

        public async Task<User> LoginAsync(LoginFormModel loginForm)
        {
            User u = default;
            if (loginForm.UserName == "admin" && loginForm.Password == "admin")
            {
                u = await userRepository.GetSingleAsync(null);
            }
            return u;
        }
    }
}
