using Project.Models.Forms;
using Project.Services.interfaces;

namespace BlazorWebAdmin.Store
{
    public class UserStore : StoreBase
    {
        private readonly ILoginService loginService;
        public UserStore(ILoginService loginService, AuthenticationStateProvider)
        {
            this.loginService = loginService;
        }

        public List<string> Roles { get; set; }
        public string UserId { get; set; }

        public async Task ReLogin(string username)
        {

        }
        public async Task<string> LoginAsync(LoginFormModel loginForm)
        {
            var flag = await loginService.LoginAsync(loginForm.UserName, loginForm.Password);
            if (flag.Success)
            {
                UserId = loginForm.UserName;
                return null;
            }
            return flag.Message;
        }
    }
}
