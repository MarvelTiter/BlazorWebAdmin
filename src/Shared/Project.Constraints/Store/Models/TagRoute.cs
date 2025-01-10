using Microsoft.AspNetCore.Components;
using Project.Constraints.PageHelper;

namespace Project.Constraints.Store.Models;

public class TagRoute : RouterMeta
{
    public bool Closable { get; set; } = true;
    public DateTime StartTime { get; set; } = DateTime.Now;
    public DateTime ActiveTime { get; set; }
    public RenderFragment? Body { get; set; }
    public RenderFragment? Title { get; set; }
    public object? PageRef { get; set; }
    public bool IsActive { get; set; }
    public bool Rendered { get; set; }
    public bool Panic { get; set; }
    //private bool isActive;
    //public bool IsActive { get => isActive; set => SetActive(value); }

    //public async void SetActive(bool active)
    //{
    //    if (active) ActiveTime = DateTime.Now;
    //    if (isActive != active && PageRef is IPageAction page)
    //    {
    //        if (active)
    //        {
    //            await page.OnShowAsync();
    //        }
    //        else
    //        {
    //            await page.OnHiddenAsync();
    //        }
    //    }
    //    isActive = active;
    //}
}

public static class TagRouteHelper
{
    public static async void SetActive(this TagRoute route, bool active)
    {
        if (active) route.ActiveTime = DateTime.Now;
        if (route.IsActive != active && route.PageRef is IPageAction page)
        {
            if (active)
            {
                await page.OnShowAsync();
            }
            else
            {
                await page.OnHiddenAsync();
            }
        }
        route.IsActive = active;
    }

    public static void Drop(this TagRoute route)
    {
        route.Body = null;
        route.Title = null;
        route.PageRef = null;
        route.Rendered = false;
    }

    //public static void OccurException(this TagRoute? route, Exception exception)
    //{
    //    if (route is )
    //}
}
