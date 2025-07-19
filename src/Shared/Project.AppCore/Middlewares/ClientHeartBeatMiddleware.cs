using AutoInjectGenerator;
using Microsoft.AspNetCore.Http;
using Project.Constraints.Utils;

namespace Project.AppCore.Middlewares;

[AutoInjectSelf(LifeTime = InjectLifeTime.Singleton)]
public class ClientHeartBeatMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        context.User.GetCookieClaimsIdentity(out var identity);
        var user = identity?.GetUserInfo() ?? UserInfo.Visitor;
        var clientIp = GetClientIp();
        var clientService = context.RequestServices.GetRequiredService<IClientService>();

        var id = context.Request.Query["id"];
        var agent = context.Request.Headers.UserAgent;

        await clientService.AddOrUpdateAsync(new()
        {
            CircuitId = id,
            IpAddress = clientIp,
            UserInfo = user,
            UserAgent = agent
        });

        return;

        string GetClientIp()
        {
            var headers = context.Request.Headers;
            // 检查多个可能的代理头
            var proxyHeaders = new[] { "X-Forwarded-For", "X-Real-IP", "CF-Connecting-IP" };
            foreach (var header in proxyHeaders)
            {
                if (headers.TryGetValue(header, out var value))
                {
                    var ips = value.ToString().Split(',', StringSplitOptions.RemoveEmptyEntries);
                    if (ips.Length > 0)
                    {
                        return ips[0].Trim();
                    }
                }
            }

            // 回退到连接远程IP
            return context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        }
    }
}