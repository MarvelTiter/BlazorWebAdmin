using AutoGenMapperGenerator;

namespace Project.Constraints.Models.Permissions;

[LangName("RolePermission")]
public interface IRolePermission
{
    [NotNull] string? RoleId { get; set; }
    [NotNull] string? PermissionId { get; set; }
}
#if (ExcludeDefaultService)
#else
[LightTable(Name = "ROLE_POWER")]
[GenMapper]
public partial class RolePermission : IRolePermission, IAutoMap
{
    [LightColumn(Name = "ROLE_ID", PrimaryKey = true)]
    [NotNull] public string? RoleId { get; set; }
    [LightColumn(Name = "POWER_ID", PrimaryKey = true)]
    [NotNull] public string? PermissionId { get; set; }
}
#endif