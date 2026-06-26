using System.Reflection;
using Project.Constraints.Store.Models;
using Project.Constraints.Utils;

namespace Project.Web.Shared.Routers;

public partial class RouterStore
{
    private readonly AsyncHandlerManager<RouteTag, bool> routerChangingHandlerManager = new();

    public IDisposable RegisterRouterChangingHandler(Func<RouteTag, Task<bool>> handler)
    {
        return routerChangingHandlerManager.RegisterHandler(handler);
    }

    private async Task<bool> OnRouterChangingAsync(RouteTag tag)
    {
        bool enable = true;
        if (IsUserDashboard(tag))
        {
            enable = EnableShowUserDashboard(userStore, setting.CurrentValue);
        }

        bool pass = true;
        await routerChangingHandlerManager.NotifyInvokeHandlers(tag, (_, newValue) =>
        {
            pass = pass && newValue;
            return pass;
        });

        return enable && pass;
    }

    private readonly AsyncHandlerManager<RouteMeta, bool> routerMetaFilterHandlerManager = new();

    public IDisposable RegisterRouterMetaFilterHandler(Func<RouteMeta, Task<bool>> handler)
    {
        return routerMetaFilterHandlerManager.RegisterHandler(handler);
    }

    private async Task<bool> OnRouteMetaFilterAsync(RouteMeta meta)
    {

        bool pass = true;
        await routerMetaFilterHandlerManager.NotifyInvokeHandlers(meta, (_, newValue) =>
        {
            pass = pass && newValue;
            return pass;
        });

        if (IsUserDashboard(meta))
        {
            return EnableShowUserDashboard(userStore, setting.CurrentValue) && pass;
        }

        var used = meta.RouteType == null
                   || AppConst.AllAssemblies.Contains(meta.RouteType.Assembly)
                   || meta.RouteType.Assembly == Assembly.GetEntryAssembly()
                   || (meta.RouteType.Assembly.GetName().Name?.EndsWith(".Client") ?? false);

        return used && pass;
    }
}