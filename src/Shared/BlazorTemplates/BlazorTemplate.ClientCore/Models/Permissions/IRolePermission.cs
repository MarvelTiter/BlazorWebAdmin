using AutoGenMapperGenerator;

namespace BlazorTemplate.ClientCore.Models.Permissions;

[LangName("RolePermission")]
public interface IRolePermission
{
    [NotNull] string? RoleId { get; set; }
    [NotNull] string? PermissionId { get; set; }
}

[LightTable(Name = "ROLE_POWER")]
[GenMapper]
public partial class TemplateRolePermission : IRolePermission
{
    [LightColumn(Name = "ROLE_ID", PrimaryKey = true)]
    [NotNull] public string? RoleId { get; set; }
    [LightColumn(Name = "POWER_ID", PrimaryKey = true)]
    [NotNull] public string? PermissionId { get; set; }
}
