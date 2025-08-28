using Project.Constraints.Store.Models;

namespace Project.Web.Shared.Routers;

public static class RouterStoreExtensions
{
    public static bool CompareUrl(this IRouterStore store, string? url)
    {
        return CompareUrl(store.CurrentUrl, url);
    }

    public static bool Compare(this IRouterStore store, RouteTag other)
    {
        return store.Current == other;
    }

    public static bool CompareUrl(string? url1, string? url2)
    {
        if (string.IsNullOrEmpty(url1) || string.IsNullOrEmpty(url2)) return false;
        return string.Equals(NormalizeUrl(url1), NormalizeUrl(url2));

        static string NormalizeUrl(string url)
        {
            if (!url.StartsWith('/'))
                url = '/' + url;
            // 统一解码
            return Uri.UnescapeDataString(url);
        }
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