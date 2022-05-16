using MDbEntity.Attributes;
using Project.Common.Attributes;

namespace Project.Models.Entities
{
    [TableName("USER")]
    public class User
    {
        [TableHeader("用户ID", 0)]
        [ColumnName("USER_ID")]
        public string UserId { get; set; }
        [TableHeader("用户名", 1)]
        [ColumnName("USER_NAME")]
        public string UserName { get; set; }
        [TableHeader("用户密码", 2)]
        [ColumnName("PASSWORD")]
        public string Password { get; set; }
    }
}
