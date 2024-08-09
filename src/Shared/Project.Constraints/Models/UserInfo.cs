using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Constraints.Models
{
    public partial class UserInfo
    {
        public string UserId { get; set; } = string.Empty;
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public object? Payload { get; set; }

        /// <summary>
        /// 框架内Api
        /// </summary>
        [NotNull] public string? Token { get; set; }
        public string[] Roles { get; set; } = [];
        /// <summary>
        /// 存储用户额外信息
        /// </summary>
        public Dictionary<string, object?> AdditionalValue { get; set; } = [];
        public DateTime CreatedTime { get; set; } = DateTime.Now;
        public DateTime ActiveTime { get; set; } = DateTime.Now;
    }
}
