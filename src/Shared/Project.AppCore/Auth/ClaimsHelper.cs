﻿using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Project.AppCore.Auth;

/// <summary>
/// 提供用于处理用户声明的辅助方法的静态类。
/// </summary>
public static class ClaimsHelper
{
    /// <summary>
    /// 根据用户信息构建一个<see cref="ClaimsPrincipal"/>对象。
    /// </summary>
    /// <param name="info">包含用户信息的<see cref="UserInfo"/>对象。</param>
    /// <returns>一个<see cref="ClaimsPrincipal"/>对象，包含根据<paramref name="info"/>生成的声明。</returns>
    public static ClaimsPrincipal BuildClaims(this UserInfo info)
    {
        // 初始化一个声明列表，包含用户的必要信息
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, info.UserId),
            new(ClaimTypes.GivenName, info.UserName!),
            new(nameof(UserInfo.UserPowers), JsonSerializer.Serialize(info.UserPowers)),
            new(nameof(UserInfo.Token), info.Token ?? ""),
            new(nameof(UserInfo.CreatedTime), $"{info.CreatedTime.ToBinary()}"),
            new(nameof(UserInfo.AdditionalValue), JsonSerializer.Serialize(info.AdditionalValue))
        };
        // 遍历用户角色，为每个角色添加一个声明
        claims.AddRange(info.Roles.Select(r => new Claim(ClaimTypes.Role, r)));

        // 遍历用户权限，为每个权限添加一个声明
        claims.AddRange((info.UserPowers ?? []).Select(p => new Claim("Right", p)));

        // 返回一个新的ClaimsPrincipal对象，包含所有声明
        return new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme));
    }

    /// <summary>
    /// 从<see cref="ClaimsPrincipal"/>对象中提取用户信息。
    /// </summary>
    /// <param name="principal">包含用户声明的<see cref="ClaimsPrincipal"/>对象。</param>
    /// <returns>一个<see cref="UserInfo"/>对象，包含从<paramref name="principal"/>提取的用户信息。</returns>
    public static UserInfo GetUserInfo(this ClaimsPrincipal principal)
    {
        // 从声明中提取用户信息
        var name = principal.Identity?.Name!;
        var username = principal.FindFirstValue(ClaimTypes.GivenName);
        var token = principal.FindFirstValue(nameof(UserInfo.Token));
        var createdBinary = principal.FindFirstValue(nameof(UserInfo.CreatedTime));
        var createdTime = DateTime.FromBinary(long.Parse(createdBinary!));
        var additionalValue = principal.FindFirstValue(nameof(UserInfo.AdditionalValue)) ?? "{}";
        var roles = principal.FindAll(ClaimTypes.Role).Select(c => c.Value);
        var powers = principal.FindAll("Right").Select(c => c.Value);

        // 构建并返回一个UserInfo对象
        return new UserInfo
        {
            UserId = name,
            UserName = username,
            Token = token,
            CreatedTime = createdTime,
            Roles = [.. roles],
            UserPowers = [..powers],
            AdditionalValue = JsonSerializer.Deserialize<Dictionary<string, object?>>(additionalValue) ?? []
        };
    }
}