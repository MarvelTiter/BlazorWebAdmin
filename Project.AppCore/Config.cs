using Microsoft.AspNetCore.Components;
using Project.Constraints.UI.Extensions;
using System.Reflection;

namespace Project.AppCore;

public class Config
{
    public static AppInfo App { get; set; } = new AppInfo();
    public static DbTableType TypeInfo { get; set; } = new DbTableType();

    static List<Assembly> PageAssemblies = new List<Assembly>();

    public static RenderFragment Footer { get; set; }
    public static List<Assembly> Pages => PageAssemblies;
    public static void SetFooter(string html)
    {
        Footer = html.AsMarkupString();
    }
    public static void AddAssembly(params Type[] types)
    {
        foreach (var item in types)
        {
            PageAssemblies.Add(item.Assembly);
        }
    }

    public static void AddAssembly(params Assembly[] asms)
    {
        foreach (var item in asms)
        {
            PageAssemblies.Add(item);
        }
    }
}
public class AppInfo
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Company { get; set; }
    public string Version { get; set; }
}
