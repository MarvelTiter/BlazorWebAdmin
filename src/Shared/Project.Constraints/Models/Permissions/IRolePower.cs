using AutoGenMapperGenerator;

namespace Project.Constraints.Models.Permissions;

[LangName("RolePower")]
public interface IRolePower
{
    [NotNull] string? RoleId { get; set; }
    [NotNull] string? PowerId { get; set; }
}
#if (ExcludeDefaultService)
#else
[LightTable(Name = "ROLE_POWER")]
[GenMapper]
public partial class RolePower : IRolePower, IAutoMap
{
    [LightColumn(Name = "ROLE_ID", PrimaryKey = true)]
    [NotNull] public string? RoleId { get; set; }
    [LightColumn(Name = "POWER_ID", PrimaryKey = true)]
    [NotNull] public string? PowerId { get; set; }
}
#endif