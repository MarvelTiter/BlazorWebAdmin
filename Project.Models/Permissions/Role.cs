using MDbEntity.Attributes;
using Project.Common.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Models.Permissions
{
    [TableName("ROLE")]
    public class Role
    {
        [TableHeader("角色编号")]
        [ColumnName("ROLE_ID")]
        public string RoleId { get; set; }
        [TableHeader("角色名称")]
        [ColumnName("ROLE_NAME")]
        public string RoleName { get; set; }
    }
}
