using Project.Constraints.Store.Models;

namespace Project.Web.Shared.Routers;

public static class RouterStoreExtensions
{
    public static bool CompareUrl(this IRouterStore store, string? url)
    {
        return CompareUrl(store.CurrentUrl, url);
    }

    public static bool Compare(this IRouterStore store, TagRoute other)
    {
        return store.Current == other;
    }

    public static bool CompareUrl(string? url1, string? url2)
    {
        if (string.IsNullOrEmpty(url1) || string.IsNullOrEmpty(url2)) return false;
        if (!url1.StartsWith('/')) url1 = '/' + url1;
        if (!url2.StartsWith('/')) url2 = '/' + url2;
        return url1 == url2;
    }

    public static void NavigateToPreiousPage(this IRouterStore store)
    {
        if (store.Current is null) return;
        var currentIndex = store.TopLinks.IndexOf(store.Current);
        if (currentIndex == 0) return;
        var previousUri = store.TopLinks[currentIndex - 1].RouteUrl;
        store.GoTo(previousUri);
    }

    public static void NavigateToNextPage(this IRouterStore store)
    {
        if (store.Current is null) return;
        var currentIndex = store.TopLinks.IndexOf(store.Current);
        if (currentIndex == store.TopLinks.Count - 1) return;
        var nextUri = store.TopLinks[currentIndex + 1].RouteUrl;
        store.GoTo(nextUri);
    }
}