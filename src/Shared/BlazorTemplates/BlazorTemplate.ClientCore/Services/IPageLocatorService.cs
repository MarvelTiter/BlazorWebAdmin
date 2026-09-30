using System.Collections.Concurrent;

namespace BlazorTemplate.ClientCore.Services;

public interface IPageLocatorService
{
    void SetPage<T>(string key);
    void SetPage(string key, Type type);
    Type? GetPage(string key);
}

public class PageLocatorService : IPageLocatorService
{
    private readonly ConcurrentDictionary<string, Type> pages = new();

    public Type? GetPage(string key)
    {
        if (pages.TryGetValue(key, out var type)) return type;
        return null;
    }

    public void SetPage<T>(string key)
    {
        SetPage(key, typeof(T));
    }

    public void SetPage(string key, Type type)
    {
        pages.TryRemove(key, out var _);
        pages.TryAdd(key, type);
    }
}

public static class PageLocatorServiceExtensions
{
    private const string RUNLOG_INDEX_PAGE_KEY = "RUNLOG_INDEX";
    private const string SYSTEM_LOGIN_PAGE_KEY = "SYSTEM_LOGIN";
    private const string USER_INDEX_PAGE_KEY = "USER_INDEX";
    private const string SYSTEM_DASHBOARD_PAGE_KEY = "SYSTEM_DASHBOARD";
    private const string PERMISSION_INDEX_PAGE_KEY = "PERMISSION_INDEX";
    private const string ROLE_PERMISSION_INDEX_PAGE_KEY = "ROLE_PERMISSION_INDEX";
    private const string DICT_INDEX_PAGE_KEY = "DICT_INDEX";

    extension(IPageLocatorService locator)
    {
        public Type? GetLoginPageType() => locator.GetPage(SYSTEM_LOGIN_PAGE_KEY);
        public void SetLoginPageType<T>() => locator.SetPage<T>(SYSTEM_LOGIN_PAGE_KEY);
        public Type? GetUserPageType() => locator.GetPage(USER_INDEX_PAGE_KEY);
        public void SetUserPageType<T>() => locator.SetPage<T>(USER_INDEX_PAGE_KEY);
        public Type? GetDashboardType() => locator.GetPage(SYSTEM_DASHBOARD_PAGE_KEY);
        public void SetDashboardType<T>() => locator.SetPage<T>(SYSTEM_DASHBOARD_PAGE_KEY);

        public Type? GetPermissionPageType() => locator.GetPage(PERMISSION_INDEX_PAGE_KEY);

        public void SetPermissionPageType<T>() => locator.SetPage<T>(PERMISSION_INDEX_PAGE_KEY);

        public Type? GetRolePermissionPageType() => locator.GetPage(ROLE_PERMISSION_INDEX_PAGE_KEY);

        public void SetRolePermissionPageType<T>() => locator.SetPage<T>(ROLE_PERMISSION_INDEX_PAGE_KEY);

        public Type? GetRunLogPageType() => locator.GetPage(RUNLOG_INDEX_PAGE_KEY);
        public void SetRunLogPageType<T>() => locator.SetPage<T>(RUNLOG_INDEX_PAGE_KEY);
        public Type? GetDictPageType() => locator.GetPage(DICT_INDEX_PAGE_KEY);
        public void SetDictPageType<T>() => locator.SetPage<T>(DICT_INDEX_PAGE_KEY);
    }
}