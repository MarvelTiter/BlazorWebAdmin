﻿
namespace Project.Constraints.Store;

[AutoInject]
public interface IUserStore
{
    UserInfo? UserInfo { get; set; }
    IEnumerable<string> Roles { get; }
    string? UserId { get; }
    string UserDisplayName {  get; }
    void SetUser(UserInfo? userInfo);
    void ClearUser();
}
