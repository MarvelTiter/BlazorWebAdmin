
using AutoGenMapperGenerator;

namespace Project.Constraints.Models.Permissions;

[LangName("UserRole")]
public interface IUserRole
{
    [NotNull] string? UserId { get; set; }
    [NotNull] string? RoleId { get; set; }
}

#if (ExcludeDefaultService)
#else
[LightTable(Name = "USER_ROLE")]
[GenMapper]
public partial class UserRole : IUserRole, IAutoMap
{
    [LightColumn(Name = "USER_ID", PrimaryKey = true)]
    [NotNull] 
    public string? UserId { get; set; }
    [LightColumn(Name = "ROLE_ID", PrimaryKey = true)]
    [NotNull]
    public string? RoleId { get; set; }
}
#endif