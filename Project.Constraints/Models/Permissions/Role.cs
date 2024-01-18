using MDbEntity.Attributes;

namespace Project.Constraints.Models.Permissions
{
    public interface IRole
    {
        string RoleId { get; set; }
        string RoleName { get; set; }
    }

    [TableName("ROLE")]
    [Table(Name = "ROLE")]
    public class Role: IRole
    {
        [PrimaryKey]
        [ColumnDefinition(Readonly = true)]
        [ColumnName("ROLE_ID")]
        [Column(Name = "ROLE_ID", PrimaryKey = true)]
        public string RoleId { get; set; }
        [ColumnDefinition]
        [ColumnName("ROLE_NAME")]
        [Column(Name = "ROLE_NAME")]
        public string RoleName { get; set; }
    }
}
