using Project.Constraints.Store.Models;
using System.Diagnostics.CodeAnalysis;

namespace Project.Constraints.Options;
public sealed class AppSetting
{
    [NotNull] public string? AppTitle { get; set; }
    [NotNull] public string? AppShortName { get; set; }
    public LayoutMode? LayoutMode { get; set; }
    public AppRunMode RunMode { get; set; } = AppRunMode.Server;
    public string? AppLanguage { get; set; }
    public string? LauchUrl { get; set; }
    public bool LoadUnregisteredPage { get; set; }
    public bool LoadPageFromDatabase { get; set; } = true;
    public bool UseAspectProxy { get; set; }
    public bool ShowExceptionDetial { get; set; }
    public int SupportedMajorVersion { get; set; } = 75;
    public List<CameraResolution>? CameraResolutions { get; set; }
    public ClientHubOptions ClientHubOptions { get; set; } = new();

}
