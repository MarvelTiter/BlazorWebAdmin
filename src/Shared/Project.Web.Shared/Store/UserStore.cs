namespace Project.Web.Shared.Store;

[AutoInject]
public class UserStore : StoreBase, IUserStore
{
    public UserInfo? UserInfo { get; private set; }
    public string[] Roles => UserInfo?.Roles ?? [];
    public string? UserId => UserInfo?.UserId;
    public string UserDisplayName => GetUserName();

    public void SetUser(UserInfo? userInfo)
    {
        UserInfo = userInfo;
    }

    public void ClearUser()
    {
        UserInfo = null;
    }

    private string GetUserName()
    {
        return UserInfo?.UserName ?? "Unknow";
    }
    // public event Func<UserInfo, Task>? LoginSuccessEvent;

    // private Task OnLoginSuccessAsync(UserInfo info)
    // {
    //     if (LoginSuccessEvent != null)
    //         return LoginSuccessEvent(info);
    //     return Task.CompletedTask;
    // }
}