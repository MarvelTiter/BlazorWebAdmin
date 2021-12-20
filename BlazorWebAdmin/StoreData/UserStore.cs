namespace BlazorWebAdmin.StoreData
{
    public class UserStore : StoreBase
    {
        public List<string> Roles { get; set; }
        public string Token { get; set; }
        public string UserId { get; set; }
    }
}
