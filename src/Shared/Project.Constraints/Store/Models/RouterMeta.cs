using System.Diagnostics.CodeAnalysis;

namespace Project.Constraints.Store.Models
{
    public class RouterMeta : IEqualityComparer<RouterMeta>
    {
        [NotNull] public string? RouteId { get; set; }
        [NotNull] public string? RouteUrl { get; set; }
        public string? Icon { get; set; }
        [NotNull] public string? RouteTitle { get; set; }
        public string Redirect { get; set; } = "NoRedirect";
        public bool Pin { get; set; }
        public string? Group { get; set; }
        public int Sort { get; set; }
        public bool HasPageInfo { get; set; }
        public bool Cache { get; set; } = true;
        public bool ForceShowOnNavMenu { get; set; }
        [NotNull] public Type? RouteType { get; set; }
        public bool Equals(RouterMeta? x, RouterMeta? y)
        {
            return x?.RouteId == y?.RouteId;
        }

        public int GetHashCode([DisallowNull] RouterMeta obj)
        {
            return obj.RouteId.GetHashCode();
        }
    }
}
