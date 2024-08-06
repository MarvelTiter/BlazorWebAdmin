﻿using AutoInjectGenerator;
using Project.Constraints.Store;

namespace Project.Constraints.Services
{
    public interface IWebSettingInitService
    {
        void UpdateUserInfo(UserInfo info);
        void ApplyAppSetting(IAppStore app);
    }

    [AutoInject]
    public class WebSettingInitService : IWebSettingInitService
    {
        private readonly IAuthenticationService authenticationService;

        public WebSettingInitService(IAuthenticationService authenticationService)
        {
            this.authenticationService = authenticationService;
        }
        public void ApplyAppSetting(IAppStore app)
        {
            throw new NotImplementedException();
        }

        public void UpdateUserInfo(UserInfo info)
        {
            throw new NotImplementedException();
        }
    }
}
