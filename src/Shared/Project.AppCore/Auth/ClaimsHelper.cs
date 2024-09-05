using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Project.AppCore.Auth;

public static class ClaimsHelper
{
    public static ClaimsPrincipal BuildClaims(this UserInfo info)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, info.UserId),
            new(ClaimTypes.GivenName, info.UserName!),
            new(nameof(UserInfo.Token), info.Token ?? ""),
            new(nameof(UserInfo.CreatedTime), $"{info.CreatedTime.ToBinary()}"),
            new(nameof(UserInfo.AdditionalValue), JsonSerializer.Serialize(info.AdditionalValue))
        };
        foreach (var r in info.Roles) claims.Add(new Claim(ClaimTypes.Role, r));
        return new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme));
    }

    public static UserInfo GetUserInfo(this ClaimsPrincipal principal)
    {
        var name = principal.Identity?.Name!;
        var username = principal.FindFirstValue(ClaimTypes.GivenName);
        var token = principal.FindFirstValue(nameof(UserInfo.Token));
        var createdBinary = principal.FindFirstValue(nameof(UserInfo.CreatedTime));
        var createdTime = DateTime.FromBinary(long.Parse(createdBinary));
        var additionalValue = principal.FindFirstValue(nameof(UserInfo.AdditionalValue)) ?? "{}";
        var roles = principal.FindAll(ClaimTypes.Role).Select(c => c.Value);
        return new UserInfo
        {
            UserId = name,
            UserName = username,
            Token = token,
            CreatedTime = createdTime,
            Roles = [.. roles],
            AdditionalValue = JsonSerializer.Deserialize<Dictionary<string, object?>>(additionalValue) ?? []
        };
    }
}