using Microsoft.FluentUI.AspNetCore.Components;
using BlazorTemplate.Constraints.Services;
using System.Collections.Concurrent;

namespace BlazorTemplate.UI.FluentUI.Components;

public static class IconHelper
{
    private static readonly ConcurrentDictionary<string, Icon> fluentIcons = new();
    public static async Task<Icon> GetCustomIcon(this ISvgIconService service, string name, IconVariant variant = IconVariant.Regular, IconSize size = IconSize.Custom)
    {
        if (!fluentIcons.TryGetValue(name, out var icon))
        {
            var svgPath = service.GetIcon(name);
            icon = new Icon(name, variant, size, svgPath?.OriginalContent ?? string.Empty);
            fluentIcons[name] = icon;
        }
        return icon;
    }
}