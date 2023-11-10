using Microsoft.Extensions.Options;
using Project.AppCore.Store;

namespace Project.AppCore.Options
{
    public sealed class AppSetting
    {
        public string AppTitle { get; set; }
        public string AppShortName { get; set; }
        public LayoutMode? LayoutMode { get; set; }
        public string? AppLanguage { get; set; }
        public string? LauchUrl { get; set; }
        public bool LoadUnregisteredPage { get; set; }
    }
}
