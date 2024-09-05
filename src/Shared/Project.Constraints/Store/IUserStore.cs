namespace Project.Constraints.Store;

public interface IUserStore
{
    // event Func<UserInfo, Task> LoginSuccessEvent;
    UserInfo? UserInfo { get; }
    string[] Roles { get; }
    string? UserId { get; }
    string UserDisplayName { get; }
    void SetUser(UserInfo? userInfo);
    void ClearUser();
}