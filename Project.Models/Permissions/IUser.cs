namespace Project.Models.Permissions
{
	public interface IUser
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
    }

    public class UserPwd
    {
        public string OldPassword { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }
}
