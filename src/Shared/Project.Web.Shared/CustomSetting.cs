using Microsoft.Extensions.DependencyInjection;
using Project.Constraints.Store.Models;
using Project.Web.Shared.Components;

namespace Project.Web.Shared;

public class CustomSetting : BasicSetting, IProjectSettingService
{
    private readonly IWatermarkServiceFactory watermarkServiceFactory;
    private readonly IUserStore userStore;

    public CustomSetting(IWatermarkServiceFactory watermarkServiceFactory
        , IUserStore userStore
        , IServiceProvider services) : base(services)
    {
        this.watermarkServiceFactory = watermarkServiceFactory;
        this.userStore = userStore;
    }

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

    public override Task<bool> RouterChangingAsync(TagRoute route)
    {
        var service = watermarkServiceFactory.GetWatermarkService();
        service.UpdateWaterMarkAsync(CurrentUser?.UserName!, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), route.RouteTitle);
        return Task.FromResult(true);
    }
}
