using Microsoft.FluentUI.AspNetCore.Components;
using Microsoft.JSInterop;
using Project.Constraints.Models;
using Project.Constraints.Page;
using Project.Constraints.Services;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.UI.FluentUI.Components
{
    public static class IconHelper
    {
        private static readonly ConcurrentDictionary<string, Icon> fluentIcons = new();
        public static async Task<Icon> GetCustomIcon(this ISvgIconService service, string name, IconVariant variant = IconVariant.Regular, IconSize size = IconSize.Custom)
        {
            if (!fluentIcons.TryGetValue(name, out var icon))
            {
                var svgPath = await service.GetIcon(name);
                icon = new Icon(name, variant, size, svgPath.Payload ?? string.Empty);
                fluentIcons[name] = icon;
            }
            return icon;
        }
    }
}
