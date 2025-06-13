using Microsoft.AspNetCore.Components;

namespace Project.Constraints.PageHelper;

/// <summary>
/// 弃用, 改用<see cref="IRoutePage"/>
/// </summary>
[Obsolete]
public interface IPageAction
{
    //RenderFragment GetTitle();
    Task OnShowAsync();
    Task OnHiddenAsync();
}

public interface IRoutePage
{
    string GetTitle() => string.Empty;
    Task OnCloseAsync() => Task.CompletedTask;
}
/// <summary>
/// 弃用, 改用<see cref="IRoutePage"/>
/// </summary>
[Obsolete]
public interface IRoutePageTitle
{
    string GetTitle();
}
