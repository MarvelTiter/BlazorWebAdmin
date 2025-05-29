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
        var agent = context.Request.Headers["UserAgent"];
        
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
            string ip;
            var headers = context.Request.Headers;
            if (headers.TryGetValue("X-Forwarded-For", out var value))
            {
                ip = value.ToString().Split(',')
                    .FirstOrDefault(s => !string.IsNullOrWhiteSpace(s))?
                    .Trim();
            }
            else
            {
                ip = context.Connection.RemoteIpAddress.ToIpString();
            }

            return ip;
        }
    }
}