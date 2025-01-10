using Microsoft.AspNetCore.Components;

namespace Project.Constraints.PageHelper;

public interface IPageAction
{
    //RenderFragment GetTitle();
    Task OnShowAsync();
    Task OnHiddenAsync();
}

public interface IRoutePageTitle
{
    RenderFragment GetTitle();
}
