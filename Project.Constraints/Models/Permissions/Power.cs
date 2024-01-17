using MDbEntity.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Project.Constraints.Models.Permissions
{
    public enum PowerType
    {
        [Display(Name = "页面")]
        Page,
        [Display(Name = "按钮")]
        Button
    }

    [Table(Name = "POWERS")]
    public class Power
    {
        [ColumnDefinition]
        [Required]
        [Column(Name = "POWER_ID", PrimaryKey = true)]
        public string PowerId { get; set; }
        [ColumnDefinition]
        [Required]
        [Column(Name = "POWER_NAME")]
        public string PowerName { get; set; }
        [ColumnDefinition]
        [Required]
        [Column(Name = "PARENT_ID")]
        public string ParentId { get; set; }
        [ColumnDefinition]
        [Required]
        [Column(Name = "POWER_TYPE")]
        public PowerType PowerType { get; set; }

        [ColumnDefinition]
        [Column(Name = "POWER_LEVEL")]
        [Form(Hide = true)]
        public int PowerLevel { get; set; }

        [ColumnDefinition]
        [Column(Name = "ICON")]
        public string Icon { get; set; }
        [ColumnDefinition]
        [Column(Name = "PATH")]
        public string Path { get; set; }
        [ColumnDefinition]
        [Column(Name = "SORT")]
        [Form(Hide = true)]
        public int Sort { get; set; }

        [Ignore]
        public IEnumerable<Power> Children { get; set; }

        [Ignore]
        [Form]
        public bool GenerateCRUDButton { get; set; }
    }
}
