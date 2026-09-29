using BlazorTemplate.ClientCore.Services;
using Microsoft.FluentUI.AspNetCore.Components;
using System.Collections.Concurrent;

namespace BlazorTemplate.UI.FluentUI.Components;

public static class IconHelper
{
    private static readonly ConcurrentDictionary<string, Icon> fluentIcons = new();
    public static Icon GetCustomIcon(this ISvgIconService service, string name, IconVariant variant = IconVariant.Regular, IconSize size = IconSize.Custom)
    {
        // 优先命中生成器产出的强类型 Icon（编译期 switch 分发，零字符串匹配、零运行时反射）
        var generated = FluentIcons.Get(name);
        if (generated is not null)
        {
            if (size != IconSize.Custom && generated is SvgIconBase svgBase)
            {
                // Icon.Size 是 init 不可改，用 WithSize 返回「同类型、新尺寸」的实例，保留 viewBox 与 ToMarkup 重写。
                return svgBase.WithSize(size);
            }
            return generated;
        }

        // 回退：非 svg- 前缀的图标（如运行时注册的自定义图标）走运行时构造
        return fluentIcons.GetOrAdd(name, key =>
        {
            var svgPath = service.GetIcon(key);
            return new Icon(key, variant, size, svgPath?.InnerContent ?? $"<i class=\"{key}\"></i>");
        });
    }
}