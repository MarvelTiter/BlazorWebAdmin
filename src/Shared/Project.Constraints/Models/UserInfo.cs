namespace Project.Constraints.Models;

public class UserInfo
{
    public string UserId { get; set; } = string.Empty;
    public string? UserName { get; set; }
    public string PasswordHash { get; set; } = string.Empty;
    /// <summary>
    /// 框架内Api
    /// </summary>
    [NotNull]
    public string? Token { get; set; }

    public string[] Roles { get; set; } = [];
    public string[] UserPowers { get; set; } = [];
    public string[] UserPages { get; set; } = [];
    /// <summary>
    /// 存储用户额外信息
    /// </summary>
    public Dictionary<string, object?> AdditionalValue { get; set; } = [];

    public DateTime CreatedTime { get; set; } = DateTime.Now;
    public bool IsVistor { get; private set; } = false;
    public static UserInfo Visitor => new()
    {
        UserId = $"{Guid.NewGuid():N}",
        UserName = "游客",
        IsVistor = true
    };
}