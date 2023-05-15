using Project.AppCore.Store;
using Project.Models.Permissions;

namespace Project.AppCore.Services
{
    public interface IWebSettingInitService
    {
        void UpdateUserInfo(UserInfo info);
        void ApplyAppSetting(AppStore app);
    }

    public class WebSettingInitService : IWebSettingInitService
    {
        private readonly ILoginService loginService;

        public WebSettingInitService(ILoginService loginService)
        {
            this.loginService = loginService;
        }
        public void ApplyAppSetting(AppStore app)
        {
            throw new NotImplementedException();
        }

        public void UpdateUserInfo(UserInfo info)
        {
            throw new NotImplementedException();
        }
    }
}
