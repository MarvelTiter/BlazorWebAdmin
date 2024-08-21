using Project.Constraints.Store;

namespace Project.Constraints.Models;

public class ClientInfo
{
    //[ColumnDefinition("Id", width: "50", Ellipsis = true)]
    public string? CircuitId { get; set; }

    [ColumnDefinition("用户ID", width: "100")]
    public string? UserId => UserStore?.UserInfo?.UserId ?? "vistor";

    [ColumnDefinition("用户姓名", width: "100")]
    public string? UserName => UserStore?.UserInfo?.UserName ?? "游客";

    [ColumnDefinition("IP", width: "150")] 
    public string? IpAddress { get; set; }

    [ColumnDefinition("接入时间", width: "150")]
    public DateTime CreateTime { get; set; } = DateTime.Now;
    public DateTime BeatTime { get; set; } = DateTime.Now;
    public IUserStore? UserStore { get; set; }

    [ColumnDefinition("UserAgent", Ellipsis = true)]
    public string? UserAgent { get; set; }
}