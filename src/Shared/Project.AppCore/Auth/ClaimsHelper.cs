using Microsoft.AspNetCore.Authentication.Cookies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Project.AppCore.Auth
{
    public static class ClaimsHelper
    {
        public static ClaimsPrincipal BuildClaims(this UserInfo info)
        {

            var claims = new List<Claim>
            {
                new (ClaimTypes.Name, info.UserId),
                new (ClaimTypes.GivenName, info.UserName!),
                new(nameof(UserInfo.Token), info.Token ?? ""),
                new(nameof(UserInfo.AdditionalValue), System.Text.Json.JsonSerializer.Serialize(info.AdditionalValue))
            };
            foreach (var r in info.Roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, r));
            }
            return new(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme));
        }

        public static UserInfo GetUserInfo(this ClaimsPrincipal principal)
        {
            var name = principal.Identity?.Name!;
            var username = principal.FindFirstValue(ClaimTypes.GivenName);
            var token = principal.FindFirstValue(nameof(UserInfo.Token));
            var additionalValue = principal.FindFirstValue(nameof(UserInfo.AdditionalValue)) ?? "{}";
            var roles = principal.FindAll(ClaimTypes.Role).Select(c => c.Value);
            return new UserInfo()
            {
                UserId = name,
                UserName = username,
                Token = token,
                Roles = [.. roles],
                AdditionalValue = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object?>>(additionalValue) ?? []
            };
        }
    }
}
