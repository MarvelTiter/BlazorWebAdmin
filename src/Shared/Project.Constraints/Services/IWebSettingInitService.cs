using AutoInjectGenerator;
using Project.Constraints.Store;

namespace Project.Constraints.Services;

public interface IWebSettingInitService
{
    void UpdateUserInfo(UserInfo info);
    void ApplyAppSetting(IAppStore app);
}

public interface IWebSettingHandler
{
    Task SaveSetting(Action<IAppStore> action);
}

[AutoInject]
public class WebSettingInitService : IWebSettingInitService
{
    public void ApplyAppSetting(IAppStore app)
    {
        throw new NotImplementedException();
    }

    public void UpdateUserInfo(UserInfo info)
    {
        throw new NotImplementedException();
    }
}