using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders.Physical;
using Project.AppCore.Auth;
using Project.Constraints;
using Project.Constraints.Services;
using System.Text;
using System.Text.Json;

namespace Project.AppCore.Middlewares
{
    public class GetClientIpMiddleware : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var ip = "";
            var headers = context.Request.Headers;
            if (headers.TryGetValue("X-Forwarded-For", out var value))
            {
                var ips = new List<string>();
                foreach (var xf in value)
                {
                    if (!string.IsNullOrEmpty(xf))
                    {
                        ips.Add(xf);
                    }
                }
                ip = string.Join(";", ips);
            }
            else
            {
                ip = context.Connection.RemoteIpAddress.ToIpString();
            }
            await context.Response.WriteAsync(ip);
        }


    }
}
