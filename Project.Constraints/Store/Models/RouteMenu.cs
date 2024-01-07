namespace Project.Constraints.Store.Models
{
    public class RouteMenu : RouterMeta
    {
        public RouteMenu()
        {

        }
        public RouteMenu(RouterMeta meta)
        {
            RouteId = meta.RouteId;
            RouteUrl = meta.RouteUrl;
            Icon = meta.Icon;
            RouteTitle = meta.RouteTitle;
            Redirect = meta.Redirect;
            Pin = meta.Pin;
            Group = meta.Group;
            Cache = meta.Cache;
        }
        public IEnumerable<RouteMenu> Children { get; set; }
        public bool HasChildren => Children != null && Children.Any();

    }
}
