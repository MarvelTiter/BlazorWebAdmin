using System.Security.Claims;
using AutoInjectGenerator;
using Microsoft.AspNetCore.Http;

namespace Project.AppCore.Middlewares;

// [AutoInject(ServiceType = typeof(GetClientIpMiddleware), LifeTime = InjectLifeTime.Singleton)]
// public class GetClientIpMiddleware : IMiddleware
// {
//     public async Task InvokeAsync(HttpContext context, RequestDelegate next)
//     {
//         string ip;
//         var headers = context.Request.Headers;
//         if (headers.TryGetValue("X-Forwarded-For", out var value))
//         {
//             ip = value.ToString().Split(',')
//                 .FirstOrDefault(s => !string.IsNullOrWhiteSpace(s))?
//                 .Trim();
//         }
//         else
//         {
//             ip = context.Connection.RemoteIpAddress.ToIpString();
//         }
//
//         await context.Response.WriteAsync(ip);
//     }
// }