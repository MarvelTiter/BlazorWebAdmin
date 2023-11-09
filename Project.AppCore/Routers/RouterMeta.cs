namespace Project.AppCore.Routers
{
    public struct MenuGroup
    {
        public string Name { get; set; }
        public string Icon { get; set; }
    }
    public class RouterMeta
    {
        public string? Id { get; set; }
        public string RouteLink { get; set; }
        public string IconName { get; set; }
        public string RouteName { get; set; }
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
            Id = meta.Id;
            RouteLink = meta.RouteLink;
            IconName = meta.IconName;
            RouteName = meta.RouteName;
            Redirect = meta.Redirect;
            Pin = meta.Pin;
            Group = meta.Group;
            Ignore = meta.Ignore;
        }
        public IEnumerable<RouteMenu> Children { get; set; }
        public bool HasChildren => Children != null && Children.Any();

    }
}
