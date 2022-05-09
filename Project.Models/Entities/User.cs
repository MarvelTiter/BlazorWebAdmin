using Project.Common.Attributes;

namespace Project.Models.Entities
{
    public class User
    {
        [TableHeader("用户ID", 0)]
        public string UserId { get; set; }
        [TableHeader("用户名", 1)]
        public string UserName { get; set; }
        [TableHeader("用户密码", 2)]
        public string Password { get; set; }
        [TableHeader("年龄", 3)]
        public int Age { get; set; }
        [TableHeader("生日", 4)]
        public DateTime Birthday { get; set; }
    }
}
