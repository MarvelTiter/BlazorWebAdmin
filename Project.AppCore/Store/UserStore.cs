using Project.Models.Forms;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using Project.AppCore.Services;
using Project.AppCore.Auth;
using Project.Models.Permissions;

namespace Project.AppCore.Store
{
    public partial class UserStore : StoreBase
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
