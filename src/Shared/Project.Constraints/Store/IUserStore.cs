
namespace Project.Constraints.Store;

public interface IUserStore
{
    // event Func<UserInfo, Task> LoginSuccessEvent;
    UserInfo? UserInfo { get; }
    IEnumerable<string> Roles { get; }
    string? UserId { get; }
    string UserDisplayName {  get; }
    string? UserAgent { get; set; }
    string? Ip { get; set; }
    void SetUser(UserInfo? userInfo);
    void ClearUser();
}
