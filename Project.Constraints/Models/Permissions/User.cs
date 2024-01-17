using MDbEntity.Attributes;

namespace Project.Constraints.Models.Permissions
{
    [Table(Name = "USER")]
    public class User : IUser
    {
        [ColumnDefinition(Readonly = true)]
        [Column(Name = "USER_ID")]
        public string UserId { get; set; }
        [ColumnDefinition]
        [Column(Name = "USER_NAME")]
        public string UserName { get; set; }
        [ColumnDefinition]
        [Column(Name = "PASSWORD")]
        [Form(InputType.Password)]
        public string Password { get; set; }
        [ColumnDefinition]
        [Column(Name = "AGE")]
        public int? Age { get; set; }
        [ColumnDefinition]
        [Column(Name = "SIGN")]
        public string Sign { get; set; }
        [ColumnDefinition]
        [Column(Name = "LAST_LOGIN")]
        [Form(Hide = true)]
        public DateTime? LastLogin { get; set; }
    }
}
