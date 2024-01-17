using MDbEntity.Attributes;

namespace Project.Constraints.Models.Permissions
{
    [Table(Name = "ROLE_POWER")]
    [TableName("ROLE_POWER")]
    public class RolePower
    {
        [Column(Name = "ROLE_ID", PrimaryKey = true)]
        [ColumnName("ROLE_ID")]
        public string RoleId { get; set; }
        [Column(Name = "POWER_ID", PrimaryKey = true)]
        [ColumnName("POWER_ID")]
        public string PowerId { get; set; }
    }
}
