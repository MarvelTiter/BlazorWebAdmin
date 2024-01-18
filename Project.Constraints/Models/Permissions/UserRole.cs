using MDbEntity.Attributes;

namespace Project.Constraints.Models.Permissions
{
    public interface IUserRole
    {
        string UserId { get; set; }
        string RoleId { get; set; }
    }

    [Table(Name = "USER_ROLE")]
    public class UserRole: IUserRole
    {
        [Column(Name = "USER_ID", PrimaryKey = true)]
        public string UserId { get; set; }
        [Column(Name = "ROLE_ID", PrimaryKey = true)]
        public string RoleId { get; set; }
    }
}
