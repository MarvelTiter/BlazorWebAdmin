using AutoGenMapperGenerator;
using System.ComponentModel.DataAnnotations;

namespace Project.Constraints.Models.Permissions;

[LangName("User")]
[SupplyColumnDefinition]
public interface IUser
{
    [ColumnDefinition(Readonly = true)]
    [NotNull] public string? UserId { get; set; }
    [ColumnDefinition]
    [NotNull] public string? UserName { get; set; }
    [ColumnDefinition]
    [Form(InputType.Password)]
    [NotNull] public string? Password { get; set; }
    [ColumnDefinition(Visible = false)]
    public IEnumerable<string>? Roles { get; set; }
    public void OnUserSave(SaveActionType type)
    {

    }
}

public class UserPwd
{
    [Form(Hide = true)]
    [NotNull]
    public string? UserId { get; set; }
    [Required]
    [NotNull]
    [ColumnDefinition]
    [Form(InputType.Password)]
    public string? OldPassword { get; set; }
    [Required]
    [NotNull]
    [ColumnDefinition]
    [Form(InputType.Password)]
    public string? Password { get; set; }
    [Required]
    [NotNull]
    [ColumnDefinition]
    [Form(InputType.Password)]
    public string? ConfirmPassword { get; set; }
}
#if (ExcludeDefaultService)
#else
[LightTable(Name = "USER")]
[GenMapper]
public partial class User : IUser, IAutoMap
{
    [LightColumn(Name = "USER_ID", PrimaryKey = true)]
    [NotNull]
    [Required]
    public string? UserId { get; set; }
    [LightColumn(Name = "USER_NAME")]
    [NotNull]
    [Required]
    public string? UserName { get; set; }
    [LightColumn(Name = "PASSWORD")]
    [NotNull]
    [Required]
    public string? Password { get; set; }
    [ColumnDefinition]
    [LightColumn(Name = "AGE")]
    public int? Age { get; set; }
    [ColumnDefinition]
    [LightColumn(Name = "SIGN")]
    public string? Sign { get; set; }
    [ColumnDefinition]
    [LightColumn(Name = "LAST_LOGIN")]
    [Form(Hide = true)]
    public DateTime? LastLogin { get; set; }
    [ColumnDefinition]
    [LightColumn(Name = "EXPIRY_DATE")]
    public DateTime? ExpiryDate { get; set; }

    [Ignore]
    public IEnumerable<string>? Roles { get; set; }
}
#endif