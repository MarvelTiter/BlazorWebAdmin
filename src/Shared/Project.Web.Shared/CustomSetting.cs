using AutoInjectGenerator;
using Microsoft.AspNetCore.Components;
using Project.Constraints;
using Project.Constraints.Models.Permissions;
using Project.Constraints.Store;
using Project.Constraints.Store.Models;
using Project.Web.Shared.Components;

namespace Project.Web.Shared;

[AutoInject(Group = "SERVER")]
public class CustomSetting : BasicSetting, IProjectSettingService
{
    private readonly IWatermarkServiceFactory watermarkServiceFactory;
    private readonly IPermissionService permissionService;

    public CustomSetting(IWatermarkServiceFactory watermarkServiceFactory
        , IPermissionService permissionService)
    {
        this.watermarkServiceFactory = watermarkServiceFactory;
        this.permissionService = permissionService;
    }
    
    public override Task LoginSuccessAsync(UserInfo result)
    {
        Console.WriteLine($"LoginSuccessAsync: {result.UserName}");
        return base.LoginSuccessAsync(result);
    }

    public override Task AfterWebApplicationAccessed()
    {
        var service = watermarkServiceFactory.GetWatermarkService();
        service.UpdateWaterMarkAsync(CurrentUser?.UserName!, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        return Task.CompletedTask;
    }

    public override Task<bool> RouterChangingAsync(TagRoute route)
    {
        var service = watermarkServiceFactory.GetWatermarkService();
        service.UpdateWaterMarkAsync(CurrentUser?.UserName!, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), route.RouteTitle);
        return Task.FromResult(true);
    }

    public override async Task<IEnumerable<MinimalPower>> GetUserPowersAsync(UserInfo info)
    {
        var result = await permissionService.GetPowerListByUserIdAsync(info.UserId);
        return result.Payload;
    }
}
