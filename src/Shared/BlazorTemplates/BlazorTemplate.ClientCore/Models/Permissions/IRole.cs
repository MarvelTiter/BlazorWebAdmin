
using AutoGenMapperGenerator;

namespace BlazorTemplate.ClientCore.Models.Permissions;

[LangName("Role")]
[SupplyColumnDefinition]
public interface IRole
{
    [ColumnDefinition(Readonly = true)]
    [NotNull] string? RoleId { get; set; }
    [ColumnDefinition]
    [NotNull] string? RoleName { get; set; }
    [ColumnDefinition(Visible = false)]
    IEnumerable<string>? Permissions { get; set; }
}

[LightTable(Name = "ROLE")]
[GenMapper]
public partial class TemplateRole : IRole
{
    [LightColumn(Name = "ROLE_ID", PrimaryKey = true)]
    [NotNull]
    public string? RoleId { get; set; }
    [LightColumn(Name = "ROLE_NAME")]
    [NotNull]
    public string? RoleName { get; set; }

    [LightColumn(Ignore = true)]
    public IEnumerable<string>? Permissions { get; set; }
}
