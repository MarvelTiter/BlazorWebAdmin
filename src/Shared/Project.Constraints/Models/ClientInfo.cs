using Microsoft.AspNetCore.Http;
using Project.Constraints.Services;

namespace Project.Constraints.Models
{
    public class ClientInfo
    {
        [ColumnDefinition]
        public string? CircuitId { get; set; }
        [ColumnDefinition]
        public DateTime CreateTime { get; set; } = DateTime.Now;
        public UserInfo? UserInfo { get; set; }
        public HttpContext? Context { get; set; }
        [ColumnDefinition]
        public string? UserId => UserInfo?.UserId;
        [ColumnDefinition]
        public string? UserName => UserInfo?.UserName;
        [ColumnDefinition]
        public string? IpAddress => Context?.Connection.RemoteIpAddress?.ToIpString();
    }
}
