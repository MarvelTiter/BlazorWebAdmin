
namespace Project.Constraints.Store;

[AutoInject]
public interface IUserStore
{
    event Func<UserInfo, Task> LoginSuccessEvent;
    UserInfo? UserInfo { get; set; }
    IEnumerable<string> Roles { get; }
    string? UserId { get; }
    string UserDisplayName {  get; }
    string? UserAgent { get; set; }
    string? Ip { get; set; }
    Task SetUserAsync(UserInfo? userInfo);
    void ClearUser();
}
