using BlazorWebAdmin.Models;

namespace BlazorWebAdmin.Store
{
    public class UserStore : StoreBase
    {
        public List<string> Roles { get; set; }
        public IEnumerable<RouterMeta> Permissions { get; set; } = new List<RouterMeta>();
        public string Token { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
    }
}
