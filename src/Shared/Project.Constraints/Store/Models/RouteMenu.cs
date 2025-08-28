namespace Project.Constraints.Store.Models;

public record RouteMenu : RouteMeta, IRouteInfo
{
    public RouteMenu()
    {

    }
    public RouteMenu(RouteMeta meta)
    {
        RouteId = meta.RouteId;
        RouteUrl = meta.RouteUrl;
        Icon = meta.Icon;
        RouteTitle = meta.RouteTitle;
        Redirect = meta.Redirect;
        Pin = meta.Pin;
        Group = meta.Group;
        Sort = meta.Sort;
        IsGroupHeader = meta.IsGroupHeader;
    }
    public IEnumerable<RouteMenu>? Children { get; set; }
    public bool HasChildren => Children != null && Children.Any();

}