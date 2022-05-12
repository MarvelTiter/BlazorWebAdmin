using MDbEntity.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Models.Permissions
{
    [TableName("ROLE_POWER")]
    public class RolePower
    {
        [ColumnName("ROLE_ID")]
        public string RoleId { get; set; }
        [ColumnName("POWER_ID")]
        public string PowerId { get; set; }
    }
}
