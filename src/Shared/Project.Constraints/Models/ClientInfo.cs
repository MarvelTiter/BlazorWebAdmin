using Microsoft.AspNetCore.Http;
using Project.Constraints.Services;

namespace Project.Constraints.Models
{
    public class ClientInfo
    {
        [ColumnDefinition("Id", width: "200", Ellipsis = true)]
        public string? CircuitId { get; set; }
        [ColumnDefinition("接入时间", width: "130")]
        public DateTime CreateTime { get; set; } = DateTime.Now;
        public UserInfo? UserInfo { get; set; }

        [ColumnDefinition("用户ID", width: "100")]
        public string? UserId => UserInfo?.UserId;
        [ColumnDefinition("用户姓名", width: "100")]
        public string? UserName => UserInfo?.UserName;
        [ColumnDefinition("IP", width: "150")]
        public string? IpAddress { get; set; }
        [ColumnDefinition("UserAgent", Ellipsis = true)]
        public string? UserAgent { get; set; }
    }
}
