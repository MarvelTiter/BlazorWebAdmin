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
        public bool Ignore { get; set; }
        public int Sort { get; set; }
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
            Ignore = meta.Ignore;
        }
        public IEnumerable<RouteMenu> Children { get; set; }
        public bool HasChildren => Children != null && Children.Any();

    }
}
