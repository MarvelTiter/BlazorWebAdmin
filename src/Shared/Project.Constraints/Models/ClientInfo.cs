using Project.Constraints.Store;

namespace Project.Constraints.Models;

public class ClientInfo(string id)
{
    [ColumnDefinition("Id", width: "50", Ellipsis = true)]
    public string CircuitId { get; set; } = id;

    [ColumnDefinition("接入时间", width: "130")]
    public DateTime CreateTime { get; set; } = DateTime.Now;

    public DateTime? BeatTime { get; set; }
    public IUserStore? UserStore { get; set; }

    [ColumnDefinition("用户ID", width: "100")]
    public string? UserId => UserStore?.UserInfo?.UserId;

    [ColumnDefinition("用户姓名", width: "100")]
    public string? UserName => UserStore?.UserInfo?.UserName;

    [ColumnDefinition("IP", width: "150")] public string? IpAddress { get; set; }

    [ColumnDefinition("UserAgent", Ellipsis = true)]
    public string? UserAgent { get; set; }
}