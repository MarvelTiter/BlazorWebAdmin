using Microsoft.AspNetCore.Components;
using Project.Constraints.Store.Models;

namespace Project.Constraints.PageHelper;

/// <summary>
/// 弃用, 改用<see cref="IRouteTagPage"/>
/// </summary>
[Obsolete]
public interface IPageAction
{
    //RenderFragment GetTitle();
    Task OnShowAsync();
    Task OnHiddenAsync();
}

/// <summary>
/// 弃用, 改用<see cref="IRouteTagPage"/>
/// </summary>
[Obsolete]
public interface IRoutePage
{
    string? GetTitle() => null;

    void OnClose()
    {
    }
}

public interface IRouteTagPage
{
    RenderFragment? GetTitle()
    {
        return null;
    }

    //void UpdateRouteTag(Action<RouteTag> option)
    //{

    //}
}

/// <summary>
/// 弃用, 改用<see cref="IRouteTagPage"/>
/// </summary>
[Obsolete]
public interface IRoutePageTitle
{
    string GetTitle();
}