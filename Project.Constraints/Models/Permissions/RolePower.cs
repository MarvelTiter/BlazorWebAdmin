using MDbEntity.Attributes;

namespace Project.Constraints.Models.Permissions
{
    public interface IRolePower
    {
        string RoleId { get; set; }
        string PowerId { get; set; }
    }
    [Table(Name = "ROLE_POWER")]
    public class RolePower: IRolePower
    {
        [Column(Name = "ROLE_ID", PrimaryKey = true)]
        public string RoleId { get; set; }
        [Column(Name = "POWER_ID", PrimaryKey = true)]
        public string PowerId { get; set; }
    }
}
