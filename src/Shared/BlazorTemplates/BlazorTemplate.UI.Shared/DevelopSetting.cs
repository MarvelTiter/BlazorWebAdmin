using Microsoft.Extensions.DependencyInjection;
using BlazorTemplate.Constraints.Store.Models;
using BlazorTemplate.UI.Shared.Components;

namespace BlazorTemplate.UI.Shared;

public class DevelopSetting(IWatermarkServiceFactory watermarkServiceFactory
        , IUserStore userStore
        , IServiceProvider services) : BasicSetting(services), IProjectSettingService
{
    public override Task LoginSuccessAsync(UserInfo result)
    {
        Console.WriteLine($"LoginSuccessAsync: {result.UserName}");
        return base.LoginSuccessAsync(result);
    }

    private UserInfo? CurrentUser => userStore.UserInfo;
    public override async Task AfterWebApplicationAccessed()
    {
        if (userStore.UserInfo is null)
        {
            var authService = ServiceProvider.GetRequiredService<IAuthenticationStateProvider>();
            await authService.ClearState();
        }
        var service = watermarkServiceFactory.GetWatermarkService();
        await service.UpdateWaterMarkAsync(CurrentUser?.UserName!, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
    }

    public override Task<bool> RouterChangingAsync(RouteMeta route)
    {
        var service = watermarkServiceFactory.GetWatermarkService();
        service.UpdateWaterMarkAsync(CurrentUser?.UserName!, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), route.RouteTitle);
        return Task.FromResult(true);
    }
}
