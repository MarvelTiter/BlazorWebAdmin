﻿using Project.Constraints.Store.Models;
using System.Diagnostics.CodeAnalysis;

namespace Project.Constraints.Options;

public record CameraResolution
{
    public string? Name { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
}
public class OnlineUserPageSetting
{
    public string[] EnableUsers { get; set; } = [];
    public string[] EnableRoles { get; set; } = [];
}
public sealed class AppSetting
{
    [NotNull] public string? AppTitle { get; set; }
    [NotNull] public string? AppShortName { get; set; }
    public LayoutMode? LayoutMode { get; set; }
    public string? AppLanguage { get; set; }
    public string? LauchUrl { get; set; }
    public bool LoadUnregisteredPage { get; set; }
    public bool LoadPageFromDatabase { get; set; } = true;
    public bool UseAspectProxy { get; set; }
    public int SupportedMajorVersion { get; set; } = 85;
    public List<CameraResolution>? CameraResolutions { get; set; }
    public OnlineUserPageSetting OnlineUserPage { get; set; } = new();

}
