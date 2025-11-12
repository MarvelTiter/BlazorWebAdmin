using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Project.AppCore.Middlewares;
using Project.Constraints;
using Project.Constraints.Utils;
using Project.Web.Shared;
using System.Reflection;

namespace Project.AppCore;

public static class WebService
{
    public static void AddServerProject(this IHostApplicationBuilder builder, Action<ProjectSetting> action)
    {
        //ScanRazorLibraryAssembly();
        builder.Services.AddClientProject(builder.Configuration, action, out var setting);
        //var setting = new ProjectSetting();
        //action.Invoke(setting);
        if (setting.AuthServiceType is not null)
        {
            builder.Services.AddScoped(typeof(IAuthService), setting.AuthServiceType);
        }
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
        {
            options.SlidingExpiration = true;
            options.Events = new CookieAuthenticationEvents()
            {
                //OnValidatePrincipal = ValidateCookiePrincipal,
                // OnCheckSlidingExpiration = context =>
                // {
                //     Console.WriteLine($"ElapsedTime:{context.ElapsedTime} -> ShouldRenew: {context.ShouldRenew}");
                //     return Task.CompletedTask;
                // },
                OnRedirectToLogin = context =>
                {
                    if (context.Request.Path.StartsWithSegments("/api"))
                    {
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        return Task.CompletedTask;
                    }
                    context.Response.Redirect(context.RedirectUri);
                    return Task.CompletedTask;
                },
                OnRedirectToAccessDenied = context =>
                {
                    if (context.Request.Path.StartsWithSegments("/api"))
                    {
                        context.Response.StatusCode = StatusCodes.Status403Forbidden;
                        return Task.CompletedTask;
                    }
                    context.Response.Redirect(context.RedirectUri);
                    return Task.CompletedTask;
                }
            };
        });
        var useProxy = builder.Configuration.GetValue<bool>("AppSetting:UseAspectProxy");
        if (useProxy)
        {
            builder.ConfigureContainer(new AutoAopProxyGenerator.AutoAopProxyServiceProviderFactory());
        }
    }
    private static void ScanRazorLibraryAssembly()
    {
        var entry = Assembly.GetEntryAssembly();
        var additionAssemblys = entry?.GetReferencedAssemblies().Select(Assembly.Load);
        foreach (var item in additionAssemblys ?? [])
        {
            var hasPage = item.ExportedTypes.Any(t => t.GetCustomAttribute<Microsoft.AspNetCore.Components.RouteAttribute>() is not null);
            if (hasPage)
            {
                AppConst.AddAssembly(item);
            }
        }
    }
    public static void UseProject(this WebApplication app)
    {
        app.UseMiddleware<CheckBrowserEnabledMiddleware>();
        app.UseStaticFiles();
        app.UseMiddleware<RedirectToLauchUrlMiddleware>();
        app.UseWhen(ctx => ctx.Request.Path.StartsWithSegments("/api/download"), a => a.UseMiddleware<FileDownloaderMiddleware>());
        // app.UseWhen(ctx => ctx.Request.Path.StartsWithSegments("/ip.client"), a => a.UseMiddleware<GetClientIpMiddleware>());
        app.UseWhen(ctx => ctx.Request.Path.StartsWithSegments("/api")
                           && !ctx.Request.Path.StartsWithSegments("/api/download")
                           && !ctx.Request.Path.StartsWithSegments("/api/svg")
                           && !ctx.Request.Path.StartsWithSegments("/api/file")
                           && !ctx.Request.Path.StartsWithSegments("/api/hub")
                           && !ctx.Request.Path.StartsWithSegments("/api/account/login")
            , a => a.UseMiddleware<SetUserInfoMiddleware>());
        app.UseWhen(ctx => ctx.Request.Path.StartsWithSegments("/client.heart.beat"), a => a.UseMiddleware<ClientHeartBeatMiddleware>());
    }

    //private static async Task ValidateCookiePrincipal(CookieValidatePrincipalContext context)
    //{
    //    var authService = context.HttpContext.RequestServices.GetRequiredService<IAuthService>();
    //    if (context.Principal?.GetCookieClaimsIdentity(out var identity) == true)
    //    {
    //        var u = identity.GetUserInfo();
    //        var ok = await authService.CheckUserStatusAsync(u);
    //        if (!ok)
    //        {
    //            context.RejectPrincipal();
    //        }
    //    }
    //}
}