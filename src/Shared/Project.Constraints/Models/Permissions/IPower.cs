using AutoGenMapperGenerator;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Constraints.Models.Permissions;

public enum PowerType
{
    [Display(Name = "页面")]
    Page,
    [Display(Name = "按钮")]
    Button,
    [Display(Name = "接口")]
    Api,
}
[LangName("Power")]
public interface IPower
{
    [NotNull] string? PowerId { get; set; }
    [NotNull] string? PowerName { get; set; }
    string? ParentId { get; set; }
    PowerType PowerType { get; set; }
    int PowerLevel { get; set; }
    string? Icon { get; set; }
    //string? Path { get; set; }
    int Sort { get; set; }
    //bool GenerateCRUDButton { get; set; }
    IEnumerable<IPower>? Children { get; set; }
}

public class MinimalPower : IPower
{
    [NotNull] public string? PowerId { get; set; }
    [NotNull] public string? PowerName { get; set; }
    public string? ParentId { get; set; }
    public PowerType PowerType { get; set; }
    public int PowerLevel { get; set; }
    public string? Icon { get; set; }
    public string? Path { get; set; }
    public int Sort { get; set; }
    public IEnumerable<MinimalPower>? Children { get; set; }
    IEnumerable<IPower>? IPower.Children { get => Children; set => Children = value?.Cast<MinimalPower>(); }
    //public bool GenerateCRUDButton { get; set; }
}

#if (ExcludeDefaultService)
#else
[LightTable(Name = "POWERS")]
[GenMapper]
public partial class Power : IPower, IAutoMap
{
    [ColumnDefinition(Readonly = true)]
    [Required]
    [LightColumn(Name = "POWER_ID", PrimaryKey = true)]
    [NotNull]
    public string? PowerId { get; set; }
    [ColumnDefinition]
    [Required]
    [LightColumn(Name = "POWER_NAME")]
    [NotNull]
    public string? PowerName { get; set; }
    [ColumnDefinition]
    [Required]
    [LightColumn(Name = "PARENT_ID")]
    public string? ParentId { get; set; }
    [ColumnDefinition]
    [Required]
    [LightColumn(Name = "POWER_TYPE")]
    public PowerType PowerType { get; set; }

    [ColumnDefinition]
    [LightColumn(Name = "POWER_LEVEL")]
    [Form(Hide = true)]
    public int PowerLevel { get; set; }

    [ColumnDefinition]
    [LightColumn(Name = "ICON")]
    public string? Icon { get; set; }

    //[ColumnDefinition]
    //[LightColumn(Name = "PATH")]
    //public string? Path { get; set; }

    [ColumnDefinition]
    [LightColumn(Name = "SORT")]
    [Form(Hide = true)]
    public int Sort { get; set; }

    [NotMapped]
    public IEnumerable<Power>? Children { get; set; } = [];
    IEnumerable<IPower>? IPower.Children { get => Children; set => Children = value?.Cast<Power>(); }

    //[NotMapped]
    //[Form]
    //public bool GenerateCRUDButton { get; set; }
}
#endif