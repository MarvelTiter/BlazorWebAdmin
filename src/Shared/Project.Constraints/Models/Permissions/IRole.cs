﻿
namespace Project.Constraints.Models.Permissions
{
    public interface IRole
    {
        [NotNull] string? RoleId { get; set; }
        [NotNull] string? RoleName { get; set; }
        IEnumerable<string>? Powers { get; set; }
    }

    [LightTable(Name = "ROLE")]
    public class Role : IRole
    {
        [ColumnDefinition(Readonly = true)]
        [LightColumn(Name = "ROLE_ID", PrimaryKey = true)]
        [NotNull]
        public string? RoleId { get; set; }
        [ColumnDefinition]
        [LightColumn(Name = "ROLE_NAME")]
        [NotNull]
        public string? RoleName { get; set; }

        [ColumnDefinition(Visible = false)]
        [Ignore]
        public IEnumerable<string>? Powers { get; set; }
    }
}
