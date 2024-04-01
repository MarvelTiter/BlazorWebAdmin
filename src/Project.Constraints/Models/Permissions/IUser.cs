using System.ComponentModel.DataAnnotations;

namespace Project.Constraints.Models.Permissions
{
    public interface IUser
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public void OnUserSave(SaveActionType type)
        {

        }
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

    [LightTable(Name = "USER")]
    public class User : IUser
    {
        [ColumnDefinition(Readonly = true)]
        [LightColumn(Name = "USER_ID", PrimaryKey = true)]
        public string UserId { get; set; }
        [ColumnDefinition]
        [LightColumn(Name = "USER_NAME")]
        public string UserName { get; set; }
        [ColumnDefinition]
        [LightColumn(Name = "PASSWORD")]
        [Form(InputType.Password)]
        public string Password { get; set; }
        [ColumnDefinition]
        [LightColumn(Name = "AGE")]
        public int? Age { get; set; }
        [ColumnDefinition]
        [LightColumn(Name = "SIGN")]
        public string Sign { get; set; }
        [ColumnDefinition]
        [LightColumn(Name = "LAST_LOGIN")]
        [Form(Hide = true)]
        public DateTime? LastLogin { get; set; }
    }
}
