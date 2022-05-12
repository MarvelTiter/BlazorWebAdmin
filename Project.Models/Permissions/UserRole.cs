using MDbEntity.Attributes;

namespace Project.Models.Permissions
{
    [TableName("USER_ROLE")]
    public class UserRole
    {        
        [ColumnName("USER_ID")]
        public string UserId { get; set; }
        [ColumnName("ROLE_ID")]
        public string RoleId { get; set; }
    }
}
