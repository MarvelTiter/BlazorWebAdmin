
namespace Project.Constraints.Models.Permissions
{
    public interface IRole
    {
        string RoleId { get; set; }
        string RoleName { get; set; }
        IEnumerable<string> Powers { get; set; }
    }

    [LightTable(Name = "ROLE")]
    public class Role: IRole
    {
        [ColumnDefinition(Readonly = true)]
        [LightColumn(Name = "ROLE_ID", PrimaryKey = true)]
        public string RoleId { get; set; }
        [ColumnDefinition]
        [LightColumn(Name = "ROLE_NAME")]
        public string RoleName { get; set; }

        [ColumnDefinition(Visible = false)]
        [Ignore]
        public IEnumerable<string> Powers { get; set; }
    }
}
