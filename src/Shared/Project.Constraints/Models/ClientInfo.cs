using Project.Constraints.Store;

namespace Project.Constraints.Models;
public class ClientInfo
{
    //[ColumnDefinition("Id", width: "50", Ellipsis = true)]
    [NotNull] public string? CircuitId { get; set; }

    [ColumnDefinition("用户ID", width: "100")]
    public string? UserId => UserInfo?.UserId ?? "vistor";

    [ColumnDefinition("用户姓名", width: "100")]
    public string? UserName => UserInfo?.UserName ?? "游客";

    [ColumnDefinition("IP", width: "150")]
    public string? IpAddress { get; set; }

    [ColumnDefinition("接入时间", width: "150")]
    public DateTime CreateTime { get; set; } = DateTime.Now;
    [ColumnDefinition("活跃时间", width: "150")]
    public DateTime BeatTime { get; set; } = DateTime.Now;
    public UserInfo? UserInfo { get; set; }

    [ColumnDefinition("UserAgent", Ellipsis = true)]
    public string? UserAgent { get; set; }

    public void Update(ClientInfo clientInfo)
    {
        IpAddress = clientInfo.IpAddress;
        BeatTime = DateTime.Now;
        UserInfo = clientInfo.UserInfo;
        UserAgent = clientInfo.UserAgent;
    }
}