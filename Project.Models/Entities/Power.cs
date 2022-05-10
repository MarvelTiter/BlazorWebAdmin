using Project.Common.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Models.Entities
{
    public enum PowerType
    {
        [Display(Name = "页面")]
        Page,
        [Display(Name = "按钮")]
        Button
    }
    public class Power
    {
        [TableHeader("权限编号")]
        public string PowerId { get; set; }
        [TableHeader("权限名称")]
        public string PowerName { get; set; }
        [TableHeader("父级权限")]
        public string ParentId { get; set; }
        [TableHeader("权限类型")]
        public PowerType PowerType { get; set; }
        [TableHeader("图标")]
        public string Icon { get; set; }
        [TableHeader("路径")]
        public string Path { get; set; }
    }
}
