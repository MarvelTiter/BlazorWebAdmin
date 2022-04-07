﻿using Project.IServices;
using Project.Models.Forms;

namespace BlazorWebAdmin.Store
{
    public class UserStore : StoreBase
    {
        private readonly ILoginService loginService;
        public UserStore(ILoginService loginService)
        {
            this.loginService = loginService;
        }

        public List<string> Roles { get; set; }
        public string Token { get; set; }
        public string UserId { get; set; }


        public async Task<bool> LoginAsync(LoginFormModel loginForm)
        {
            var flag = await loginService.LoginAsync(loginForm.UserName, loginForm.Password, out var token);
            Token = token;
            UserId = loginForm.UserName;
            return flag;
        }
    }
}
