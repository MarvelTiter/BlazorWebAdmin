using System.Security.Claims;
using System.Text.Json;

namespace Project.Constraints.Utils;

/// <summary>
/// 提供用于处理用户声明的辅助方法的静态类。
/// </summary>
public static class ClaimsHelper
{
    const string USER_POWER = "POWER";
    const string USER_MENU = "MENU";

    /// <summary>
    /// 根据用户信息构建一个<see cref="ClaimsPrincipal"/>对象。
    /// </summary>
    /// <param name="info">包含用户信息的<see cref="UserInfo"/>对象。</param>
    /// <param name="scheme"></param>
    /// <returns>一个<see cref="ClaimsPrincipal"/>对象，包含根据<paramref name="info"/>生成的声明。</returns>
    public static ClaimsPrincipal BuildClaims(this UserInfo info, string scheme)
    {
        // 初始化一个声明列表，包含用户的必要信息
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, info.UserId),
            new(ClaimTypes.GivenName, info.UserName!),
            //new(nameof(UserInfo.UserPowers), JsonSerializer.Serialize(info.UserPowers)),
            new(nameof(UserInfo.Token), info.Token ?? ""),
            new(nameof(UserInfo.CreatedTime), $"{info.CreatedTime.ToBinary()}"),
            new(nameof(UserInfo.AdditionalValue), JsonSerializer.Serialize(info.AdditionalValue)),
            new(nameof(UserInfo.PasswordHash), info.PasswordHash)
        };
        // 遍历用户角色，为每个角色添加一个声明
        claims.AddRange(info.Roles.Select(r => new Claim(ClaimTypes.Role, r)));

        // 遍历用户权限，为每个权限添加一个声明
        // claims.AddRange(info.UserPowers.Select(p => new Claim(USER_POWER, p)));

        // 遍历用户菜单，为每个菜单添加一个声明 
        // claims.AddRange(info.UserPages.Select(p => new Claim(USER_MENU, p)));

        // 返回一个新的ClaimsPrincipal对象，包含所有声明 CookieAuthenticationDefaults.AuthenticationScheme
        return new ClaimsPrincipal(new ClaimsIdentity(claims, scheme));
    }

    /// <summary>
    /// 从<see cref="ClaimsPrincipal"/>对象中提取用户信息。
    /// </summary>
    /// <param name="principal">包含用户声明的<see cref="ClaimsPrincipal"/>对象。</param>
    /// <returns>一个<see cref="UserInfo"/>对象，包含从<paramref name="principal"/>提取的用户信息。</returns>
    public static UserInfo GetUserInfo(this ClaimsIdentity principal)
    {
        // 从声明中提取用户信息
        var name = principal.Name!;
        var username = principal.FindFirst(ClaimTypes.GivenName)!.Value;
        var token = principal.FindFirst(nameof(UserInfo.Token))!.Value;
        var pwdHash = principal.FindFirst(nameof(UserInfo.PasswordHash))?.Value;
        var createdBinary = principal.FindFirst(nameof(UserInfo.CreatedTime)).Value;
        var createdTime = DateTime.FromBinary(long.Parse(createdBinary!));
        var additionalValue = principal.FindFirst(nameof(UserInfo.AdditionalValue))?.Value ?? "{}";
        var roles = principal.FindAll(ClaimTypes.Role).Select(c => c.Value);
        // var powers = principal.FindAll(USER_POWER).Select(c => c.Value);
        // var pages = principal.FindAll(USER_MENU).Select(c => c.Value);

        // 构建并返回一个UserInfo对象
        return new UserInfo
        {
            UserId = name,
            UserName = username,
            PasswordHash = pwdHash,
            Token = token,
            CreatedTime = createdTime,
            Roles = [.. roles],
            // UserPowers = [.. powers],
            // UserPages = [.. pages],
            AdditionalValue = JsonSerializer.Deserialize<Dictionary<string, object?>>(additionalValue) ?? []
        };
    }
}