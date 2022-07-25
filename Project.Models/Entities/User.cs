using MDbEntity.Attributes;
using Project.Common.Attributes;

namespace Project.Models.Entities
{
    [Table(Name = "USER")]
    public class User
    {
        [ColumnDefinition("用户ID", 0, fix: "left", width: "100px")]
        [Column(Name = "USER_ID")]
        public string UserId { get; set; }
        [ColumnDefinition("用户名", 1, fix: "left", width: "100px")]
        [Column(Name = "USER_NAME")]
        public string UserName { get; set; }
        [ColumnDefinition("用户密码", 2, fix: "left")]
        [Column(Name = "PASSWORD")]
        public string Password { get; set; }
        [ColumnDefinition("年龄", 3)]
        [Column(Name = "AGE")]
        public int? Age { get; set; }
        [ColumnDefinition("签名", 4)]
        [Column(Name = "SIGN")]
        public string Sign { get; set; }
        [ColumnDefinition("最后登录", 5)]
        [Column(Name = "LAST_LOGIN")]
        public DateTime? LastLogin { get; set; }
    }
}
