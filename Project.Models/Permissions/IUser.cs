using System.ComponentModel.DataAnnotations;

namespace Project.Models.Permissions
{
    public interface IUser
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }

    public class UserPwd
    {
        [Required]
        public string OldPassword { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string ConfirmPassword { get; set; }
    }
}
