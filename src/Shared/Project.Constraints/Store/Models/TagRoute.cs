using Microsoft.AspNetCore.Components;
using Project.Constraints.PageHelper;

namespace Project.Constraints.Store.Models;

public class TagRoute : RouterMeta
{
    public bool Closable { get; set; } = true;
    public DateTime StartTime { get; set; } = DateTime.Now;
    public DateTime ActiveTime { get; set; }
    public RenderFragment? Title { get; set; }
    public bool IsActive { get; set; }
    public bool Panic { get; set; }
    public Exception? Exception { get; set; }
    // public bool Rendered { get; set; }
}

public static class TagRouteHelper
{
    public static void SetActive(this TagRoute route, bool active)
    {
        if (active) route.ActiveTime = DateTime.Now;
        route.IsActive = active;
        // route.Rendered = active;
    }

    public static void TrySetDisactive(this TagRoute route, WeakReference<object?> pageInstance)
    {
        route.SetActive(false);
        if (pageInstance.TryGetTarget(out var page) && page is IRoutePage rp)
        {
            Console.WriteLine($"{route.RouteTitle} -> close");
            rp.OnClose();
        }
    }

    public static void Drop(this TagRoute route)
    {
        //route.Body = null;
        route.Title = null;
        //route.PageRef = null;
    }

    //public static void OccurException(this TagRoute? route, Exception exception)
    //{
    //    if (route is )
    //}
}