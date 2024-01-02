using Project.Models.Permissions;
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

        public void SetUser(UserInfo? userInfo)
        {
            UserInfo = userInfo;
        }        

        public void ClearUser()
        {
            UserInfo = null;
        }
    }
}
