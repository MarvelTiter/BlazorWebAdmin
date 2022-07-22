using MDbEntity.Attributes;
using Project.Common.Attributes;

namespace Project.Models.Entities
{
    [Table(Name = "USER")]
    public class User
    {
        [ColumnDefinition("用户ID", 0)]
        [Column(Name = "USER_ID")]
        public string UserId { get; set; }
        [ColumnDefinition("用户名", 1)]
        [Column(Name = "USER_NAME")]
        public string UserName { get; set; }
        [ColumnDefinition("用户密码", 2)]
        [Column(Name = "PASSWORD")]
        public string Password { get; set; }
        [ColumnDefinition("最后登录", 3)]
        [Column(Name = "LAST_LOGIN")]
        public DateTime? LastLogin { get; set; }
    }
}
