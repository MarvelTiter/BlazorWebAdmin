using System.Collections.Concurrent;

namespace Project.AppCore.Routers
{
    public class RouterMeta
    {
        public string RouteId { get; set; }
        public string RouteUrl { get; set; }
        public string Icon { get; set; }
        public string RouteTitle { get; set; }
        public string Redirect { get; set; } = "NoRedirect";
        public bool Pin { get; set; }
        public string? Group { get; set; }
        public int Sort { get; set; }
        public bool HasPageInfo { get; set; }
        public bool Cache { get; set; } = true;
    }

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
