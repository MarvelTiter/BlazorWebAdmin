using Microsoft.AspNetCore.Components;
namespace Project.Constraints.Store.Models;

public interface IRouteInfo
{
    /// <summary>
    /// 通过<see cref="PageInfoAttribute"/>自定义，默认是类型名称
    /// </summary>
    string RouteId { get; }
    /// <summary>
    /// 关联<see cref="RouteAttribute"/>的值
    /// </summary>
    string RouteUrl { get; }
    /// <summary>
    /// 通过<see cref="PageInfoAttribute"/>自定义，默认是类型名称
    /// </summary>
    string RouteTitle { get; }
}

public record RouteMeta : IRouteInfo//: IEqualityComparer<RouteMeta>
{
    public RouteMeta()
    {
        Redirect = "NoRedirect";
    }
    [NotNull] public string? RouteId { get; set; }
    [NotNull] public string? RouteUrl { get; set; }
    [NotNull] public string? RouteTitle { get; set; }
    [NotNull] public Type? RouteType { get; set; }
    public string? Icon { get; set; }
    public string Redirect { get; set; }
    public bool Pin { get; set; }
    public string? Group { get; set; }
    public int Sort { get; set; }
    public bool HasPageInfo { get; set; }
    //public bool Cache { get; set; } = true;
    public bool IsGroupHeader { get; set; }
    public bool IsAllowAnonymous { get; set; }

    //public bool Equals(RouteMeta? x, RouteMeta? y)
    //{
    //    return x?.RouteId == y?.RouteId;
    //}

    //public int GetHashCode([DisallowNull] RouteMeta obj)
    //{
    //    return obj.RouteId.GetHashCode();
    //}
}