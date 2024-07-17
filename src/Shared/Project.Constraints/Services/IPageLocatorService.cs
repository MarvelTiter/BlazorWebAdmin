using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Constraints.Services
{
    
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
        public static Type? GetUserPageType(this IPageLocatorService locator) => locator.GetPage("USER_INDEX");
        public static Type? GetDashboardType(this IPageLocatorService locator) => locator.GetPage("SYSTEM_DASHBOARD");
        public static Type? GetPermissionPageType(this IPageLocatorService locator) => locator.GetPage("PERMISSION_INDEX");
        public static Type? GetRolePermissionPageType(this IPageLocatorService locator) => locator.GetPage("ROLE_PERMISSION_INDEX");
        public static Type? GetRunLogPageType(this IPageLocatorService locator) => locator.GetPage("RUNLOG_INDEX");

        public static void SetUserPageType<T>(this IPageLocatorService locator) => locator.SetPage<T>("USER_INDEX");
        public static void SetDashboardType<T>(this IPageLocatorService locator) => locator.SetPage<T>("SYSTEM_DASHBOARD");
        public static void SetPermissionPageType<T>(this IPageLocatorService locator) => locator.SetPage<T>("PERMISSION_INDEX");
        public static void SetRolePermissionPageType<T>(this IPageLocatorService locator) => locator.SetPage<T>("ROLE_PERMISSION_INDEX");
        public static void SetRunLogPageType<T>(this IPageLocatorService locator) => locator.SetPage<T>("RUNLOG_INDEX");
    }
}
