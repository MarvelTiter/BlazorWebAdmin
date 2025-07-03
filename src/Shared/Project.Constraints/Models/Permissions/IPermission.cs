using AutoGenMapperGenerator;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Constraints.Models.Permissions;

public enum PermissionType
{
    [Display(Name = "页面")]
    Page,
    [Display(Name = "按钮")]
    Button,
    [Display(Name = "接口")]
    Api,
}
[LangName("Permission")]
[SupplyColumnDefinition]
public interface IPermission
{
    [ColumnDefinition(Readonly = true)]
    [NotNull] string? PermissionId { get; set; }
    [ColumnDefinition]
    [NotNull] string? PermissionName { get; set; }
    [ColumnDefinition]
    string? ParentId { get; set; }
    [ColumnDefinition]
    PermissionType PermissionType { get; set; }
    [ColumnDefinition]
    int PermissionLevel { get; set; }
    [ColumnDefinition]
    string? Icon { get; set; }
    //string? Path { get; set; }
    [ColumnDefinition]
    [Form(Hide = true)]
    int Sort { get; set; }
    //bool GenerateCRUDButton { get; set; }
    IEnumerable<IPermission>? Children { get; set; }
}

public class MinimalPermission : IPermission
{
    [NotNull] public string? PermissionId { get; set; }
    [NotNull] public string? PermissionName { get; set; }
    public string? ParentId { get; set; }
    public PermissionType PermissionType { get; set; }
    public int PermissionLevel { get; set; }
    public string? Icon { get; set; }
    public string? Path { get; set; }
    public int Sort { get; set; }
    public IEnumerable<MinimalPermission>? Children { get; set; }
    IEnumerable<IPermission>? IPermission.Children { get => Children; set => Children = value?.Cast<MinimalPermission>(); }
    //public bool GenerateCRUDButton { get; set; }
}

#if (ExcludeDefaultService)
#else
[LightTable(Name = "POWERS")]
[GenMapper]
public partial class Permission : IPermission, IAutoMap
{
    [Required]
    [LightColumn(Name = "POWER_ID", PrimaryKey = true)]
    [NotNull]
    public string? PermissionId { get; set; }
    [Required]
    [LightColumn(Name = "POWER_NAME")]
    [NotNull]
    public string? PermissionName { get; set; }
    [Required]
    [LightColumn(Name = "PARENT_ID")]
    public string? ParentId { get; set; }
    [Required]
    [LightColumn(Name = "POWER_TYPE")]
    public PermissionType PermissionType { get; set; }

    [LightColumn(Name = "POWER_LEVEL")]
    [Form(Hide = true)]
    public int PermissionLevel { get; set; }

    [LightColumn(Name = "ICON")]
    public string? Icon { get; set; }

    //[ColumnDefinition]
    //[LightColumn(Name = "PATH")]
    //public string? Path { get; set; }

    [LightColumn(Name = "SORT")]
    public int Sort { get; set; }

    [NotMapped]
    public IEnumerable<Permission>? Children { get; set; } = [];
    IEnumerable<IPermission>? IPermission.Children { get => Children; set => Children = value?.Cast<Permission>(); }

    //[NotMapped]
    //[Form]
    //public bool GenerateCRUDButton { get; set; }
}
#endif