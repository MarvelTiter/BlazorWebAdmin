using Project.Constraints.Store.Models;
using Project.Web.Shared.Components;

namespace Project.Web.Shared;

public class CustomSetting : BasicSetting, IProjectSettingService
{
    private readonly IWatermarkServiceFactory watermarkServiceFactory;

    public CustomSetting(IWatermarkServiceFactory watermarkServiceFactory
        , IServiceProvider services) : base(services)
    {
        this.watermarkServiceFactory = watermarkServiceFactory;
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
}
