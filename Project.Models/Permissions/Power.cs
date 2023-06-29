using MDbEntity.Attributes;
using Project.Common.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Models.Permissions
{
    public enum PowerType
    {
        [System.ComponentModel.DataAnnotations.Display(Name = "页面")]
        Page,
        [System.ComponentModel.DataAnnotations.Display(Name = "按钮")]
        Button
    }

    [TableName("POWERS")]
    [Table(Name = "POWERS")]
    public class Power
    {
        [ColumnDefinition]
        [Column(Name = "POWER_ID", PrimaryKey = true)]
        public string PowerId { get; set; }
        [ColumnDefinition]
        [Column(Name = "POWER_NAME")]
        public string PowerName { get; set; }
        [ColumnDefinition]
        [Column(Name = "PARENT_ID")]
        public string ParentId { get; set; }
        [ColumnDefinition]
        [Column(Name = "POWER_TYPE")]
        public PowerType PowerType { get; set; }

        [ColumnDefinition]
        [Column(Name = "POWER_LEVEL")]
        public int PowerLevel { get; set; }

        [ColumnDefinition]
        [Column(Name = "ICON")]
        public string Icon { get; set; }
        [ColumnDefinition]
        [Column(Name = "PATH")]
        public string Path { get; set; }
        [ColumnDefinition]
        [Column(Name = "SORT")]
        public int Sort { get; set; }
    }
}
