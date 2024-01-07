using MDbEntity.Attributes;

namespace Project.Models.Entities.Permissions
{
    [TableName("USER_ROLE")]
    [Table(Name = "USER_ROLE")]
    public class UserRole
    {
        [ColumnName("USER_ID")]
        [Column(Name = "USER_ID", PrimaryKey = true)]
        public string UserId { get; set; }
        [ColumnName("ROLE_ID")]
        [Column(Name = "ROLE_ID", PrimaryKey = true)]
        public string RoleId { get; set; }
    }
}
