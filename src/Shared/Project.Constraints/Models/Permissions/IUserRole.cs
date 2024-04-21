
namespace Project.Constraints.Models.Permissions
{
    public interface IUserRole
    {
        string UserId { get; set; }
        string RoleId { get; set; }
    }

    [LightTable(Name = "USER_ROLE")]
    public class UserRole: IUserRole
    {
        [LightColumn(Name = "USER_ID", PrimaryKey = true)]
        public string UserId { get; set; }
        [LightColumn(Name = "ROLE_ID", PrimaryKey = true)]
        public string RoleId { get; set; }
    }
}
