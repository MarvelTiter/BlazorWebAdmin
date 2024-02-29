using Project.Constraints.Store.Models;

namespace Project.Constraints.Options;

public sealed class AppSetting
{
    public string AppTitle { get; set; }
    public string AppShortName { get; set; }
    public LayoutMode? LayoutMode { get; set; }
    public string? AppLanguage { get; set; }
    public string? LauchUrl { get; set; }
    public bool LoadUnregisteredPage { get; set; }
    public bool LoadPageFromDatabase { get; set; } = true;
    public bool UseAspectProxy { get; set; }
}
