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
        [PrimaryKey]
        [ColumnDefinition]
        [ColumnName("POWER_ID")]
        [Column(Name = "POWER_ID", PrimaryKey = true)]
        public string PowerId { get; set; }
        [ColumnDefinition]
        [ColumnName("POWER_NAME")]
        [Column(Name = "POWER_NAME")]
        public string PowerName { get; set; }
        [ColumnDefinition]
        [ColumnName("PARENT_ID")]
        [Column(Name = "PARENT_ID")]
        public string ParentId { get; set; }
        [ColumnDefinition]
        [ColumnName("POWER_TYPE")]
        [Column(Name = "POWER_TYPE")]
        public PowerType PowerType { get; set; }
        [ColumnDefinition]
        [ColumnName("ICON")]
        [Column(Name = "ICON")]
        public string Icon { get; set; }
        [ColumnDefinition]
        [ColumnName("PATH")]
        [Column(Name = "PATH")]
        public string Path { get; set; }
        [ColumnDefinition]
        [ColumnName("SORT")]
        [Column(Name = "SORT")]
        public int Sort { get; set; }
    }
}
