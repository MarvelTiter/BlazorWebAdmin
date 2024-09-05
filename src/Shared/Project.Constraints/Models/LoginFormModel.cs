using System.ComponentModel.DataAnnotations;

namespace Project.Constraints.Models
{
    public class LoginFormModel
    {
        [Required]
        public string UserName { get; set; } = "";
        [Required]
        public string Password { get; set; } = "";
    }

}
