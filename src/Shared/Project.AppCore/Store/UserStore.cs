using Project.Constraints.Models;
using Project.Constraints.Store;

namespace Project.AppCore.Store
{
    public partial class UserStore : StoreBase, IUserStore
    {
        public UserInfo? UserInfo { get; set; }
        public IEnumerable<string> Roles => UserInfo?.Roles;
        public string? UserId => UserInfo?.UserId;
        public string UserDisplayName => GetUserName();

        private string GetUserName()
        {
            return UserInfo?.UserName ?? "Unknow";
        }

        public async Task SetUserAsync(UserInfo? userInfo)
        {
            if (userInfo != null)
            {
                await OnLoginSuccessAsync(userInfo);
            }
            UserInfo = userInfo;
        }

        public void ClearUser()
        {
            UserInfo = null;
        }
        public event Func<UserInfo, Task> LoginSuccessEvent;

        private Task OnLoginSuccessAsync(UserInfo info)
        {
            if (LoginSuccessEvent != null)
                return LoginSuccessEvent(info);
            return Task.CompletedTask;
        }
    }
}
