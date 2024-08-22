
using AutoGenMapperGenerator;

namespace Project.Constraints.Models.Permissions
{
    public interface IUserRole
    {
        [NotNull] string? UserId { get; set; }
        [NotNull] string? RoleId { get; set; }
    }

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
}
