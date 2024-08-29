using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Hosting;
using Project.Constraints.Models.Permissions;
using Project.Constraints.UI.Extensions;
using System.Reflection;

namespace Project.Constraints;
public class AppInfo
{
    public string? Id { get; set; }
    public string? Name { get; set; }
    public string? Company { get; set; }
    public string? Version { get; set; }
}

public static class AppConst
{
    public static AppInfo App { get; set; } = new AppInfo();

    static List<Assembly> AdditionalPageAssemblies = new List<Assembly>();
    //public static Assembly ServerAssembly { get; set; }
    //public static Assembly ClientAssembly { get; set; }
    public static string GetStatisticsFileWithVersion(string path)
    {
        if (Environment?.IsDevelopment() == true)
        {
            return path;
        }
        var file = Path.Combine("wwwroot", path);
        var fi = new FileInfo(file);
        if (!fi.Exists)
        {
            return path;
        }
        return $"{path}?v={fi.LastWriteTime:yyMMddHHmmss}";
    }

    public static RenderFragment? Footer { get; set; }
    public static List<Assembly> Pages => AdditionalPageAssemblies;
    //public static IEnumerable<Assembly> AllEnableAssembly()
    //{
    //    yield return ServerAssembly;
    //    yield return ClientAssembly;
    //    foreach (var item in Pages)
    //    {
    //        yield return item;
    //    }
    //}
    public static void SetFooter(string html)
    {
        Footer = html.AsMarkupString();
    }
    public static void AddAssembly(params Type[] types)
    {
        foreach (var item in types)
        {
            AdditionalPageAssemblies.Add(item.Assembly);
        }
    }

    public static void AddAssembly(params Assembly[] asms)
    {
        foreach (var item in asms)
        {
            AdditionalPageAssemblies.Add(item);
        }
    }

    public static readonly string TempFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tempfile");
    public static string GetPath(params string[] paths)
    {
        if (paths?.Length == 0)
        {
            return TempFilePath;
        }
        return Path.Combine([AppDomain.CurrentDomain.BaseDirectory, .. paths]);
    }
    public static IHostEnvironment? Environment { get; set; }
    public static IServiceProvider? Services { get; set; }

    static AppConst()
    {
        if (!Directory.Exists(TempFilePath))
            Directory.CreateDirectory(TempFilePath);
    }
}
