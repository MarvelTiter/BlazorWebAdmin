using Project.Constraints.Store;
using Project.Models.Permissions;

namespace Project.Constraints.Services
{
	public interface IWebSettingInitService
	{
		void UpdateUserInfo(UserInfo info);
		void ApplyAppSetting(IAppStore app);
	}

	public class WebSettingInitService : IWebSettingInitService
	{
		private readonly ILoginService loginService;

		public WebSettingInitService(ILoginService loginService)
		{
			this.loginService = loginService;
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
