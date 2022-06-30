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
        [ColumnDefinition("权限ID")]
        [ColumnName("POWER_ID")]
        [Column(Name = "POWER_ID", PrimaryKey = true)]
        public string PowerId { get; set; }
        [ColumnDefinition("权限名称")]
        [ColumnName("POWER_NAME")]
        [Column(Name = "POWER_NAME")]
        public string PowerName { get; set; }
        [ColumnDefinition("父级权限")]
        [ColumnName("PARENT_ID")]
        [Column(Name = "PARENT_ID")]
        public string ParentId { get; set; }
        [ColumnDefinition("权限类型")]
        [ColumnName("POWER_TYPE")]
        [Column(Name = "POWER_TYPE")]
        public PowerType PowerType { get; set; }
        [ColumnDefinition("图标")]
        [ColumnName("ICON")]
        [Column(Name = "ICON")]
        public string Icon { get; set; }
        [ColumnDefinition("路径")]
        [ColumnName("PATH")]
        [Column(Name = "PATH")]
        public string Path { get; set; }
        [ColumnDefinition("排序")]
        [ColumnName("SORT")]
        [Column(Name = "SORT")]
        public int Sort { get; set; }
    }
}
