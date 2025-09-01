namespace Project.Constraints.Options;

public class ClientHubOptions
{
    /// <summary>
    /// 是否启用
    /// </summary>
    public bool Enable { get; set; }
    /// <summary>
    /// 服务器扫描过期客户端的频率，默认5分钟扫一次
    /// </summary>
    public TimeSpan ServerScanFrequency { get; set; } = TimeSpan.FromMinutes(5);
    /// <summary>
    /// 客户端发送心跳的频率，默认5秒一次
    /// </summary>
    public TimeSpan ClientSendFrequency { get; set; } = TimeSpan.FromSeconds(5);
    /// <summary>
    /// 心跳超时时间限制
    /// </summary>
    public TimeSpan ClearTimeoutLimit { get; set; } = TimeSpan.FromSeconds(15);
    //public string[] AllowUsers { get; set; } = [];
    //public string[] AllowRoles { get; set; } = [];
}
