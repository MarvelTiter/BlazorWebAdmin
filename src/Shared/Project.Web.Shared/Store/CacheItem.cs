using Microsoft.AspNetCore.Components;

namespace Project.Web.Shared.Store
{
    public class CacheItem
    {
        public DateTime StartTime { get; set; }
        public DateTime ActiveTime { get; set; }
        public TimeSpan LifeTime { get; set; }
        public RenderFragment? Body { get; set; }
    }
}
