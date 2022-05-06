using Project.Common.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Models.Entities
{
    public class Role
    {
        [TableHeader("角色编号")]
        public string RoleId { get; set; }
        [TableHeader("角色名称")]
        public string RoleName { get; set; }
    }
}
