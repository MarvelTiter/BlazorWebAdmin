using Microsoft.FluentUI.AspNetCore.Components;

namespace BlazorTemplate.UI.FluentUI.Components;

/// <summary>
/// 自定义 svg 图标的公共基类。Content 保存完整 <c>&lt;svg&gt;</c> 文档（含自身 viewBox），
/// 渲染交给 FluentUI 的 <c>IconSize.Custom</c> 原样输出路径（见官方 issue #1328）。
/// </summary>
public abstract class SvgIconBase : Icon
{
    /// <summary>svg 模板里的 width/height 占位符，由 <see cref="Resize"/> 在运行时替换为实际像素值。</summary>
    protected const string WidthPlaceholder = "__W__";
    protected const string HeightPlaceholder = "__H__";

    protected SvgIconBase(string name, string svgContent, IconSize size)
        : base(name, IconVariant.Regular, size, svgContent)
    {
    }

    /// <summary>返回一个指定尺寸的新实例（直接把 svg 的 width/height 改写为对应像素值）。</summary>
    public abstract Icon WithSize(IconSize size);

    /// <summary>
    /// 把 svg 模板里的 <c>__W__</c>/<c>__H__</c> 占位符替换为 <paramref name="size"/> 对应的像素值。
    /// <c>IconSize.Custom</c> 时保持 <paramref name="defaultWidth"/>（源 svg 的 width 值）。
    /// </summary>
    protected static string Resize(string svgTemplate, IconSize size, string defaultWidth)
    {
        var px = size == IconSize.Custom ? defaultWidth : ((int)size).ToString();
        return svgTemplate
            .Replace(WidthPlaceholder, px)
            .Replace(HeightPlaceholder, px);
    }
}
