using Microsoft.AspNetCore.Http;
using Project.Constraints.Services;
using Project.Constraints.Store;

namespace Project.Constraints.Models
{
    public class ClientInfo
    {
        [ColumnDefinition("Id", width: "200", Ellipsis = true)]
        public string? CircuitId { get; set; }
        [ColumnDefinition("接入时间", width: "130")]
        public DateTime CreateTime { get; set; } = DateTime.Now;
        public IUserStore? UserStore { get; set; }

        [ColumnDefinition("用户ID", width: "100")]
        public string? UserId => UserStore?.UserInfo?.UserId;
        [ColumnDefinition("用户姓名", width: "100")]
        public string? UserName => UserStore?.UserInfo?.UserName;
        [ColumnDefinition("IP", width: "150")]
        public string? IpAddress => UserStore?.Ip;
        [ColumnDefinition("UserAgent", Ellipsis = true)]
        public string? UserAgent => UserStore?.UserAgent;
    }
}
