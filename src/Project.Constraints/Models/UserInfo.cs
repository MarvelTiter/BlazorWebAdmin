using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Constraints.Models
{
    public partial class UserInfo
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public object? Payload { get; set; }
        /// <summary>
        /// 框架内Api
        /// </summary>
        public string ApiToken { get; set; }
        public IEnumerable<string> Roles { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime ActiveTime { get; set; }
        public UserInfo()
        {
            UserId = string.Empty;
            Payload = null;
            Roles = Enumerable.Empty<string>();
            CreatedTime = DateTime.Now;
            ActiveTime = DateTime.Now;
        }
    }
}
