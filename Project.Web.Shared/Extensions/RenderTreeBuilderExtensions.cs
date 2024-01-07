using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Project.Web.Shared.Extensions
{
    public static class RenderTreeBuilderExtensions
    {
        public static void MakeDiv(this RenderTreeBuilder builder, Action child) => builder.MakeDiv("", child);
        public static void MakeDiv(this RenderTreeBuilder builder, string className, string content) => builder.MakeDiv(className, () => builder.Markup(content));
        public static void MakeDiv(this RenderTreeBuilder builder, string className, Action child)
        {
            builder.OpenElement(0, "div");
            if (!string.IsNullOrEmpty(className))
                builder.Class(className);
            child?.Invoke();
            builder.CloseElement();
        }

        public static void MakeSpan(this RenderTreeBuilder builder, Action child) => builder.MakeSpan("", child);
        public static void MakeSpan(this RenderTreeBuilder builder, string className, string content) => builder.MakeSpan(className, () => builder.Markup(content));
        public static RenderTreeBuilder MakeSpan(this RenderTreeBuilder builder, string className, Action child)
        {
            builder.OpenElement(0, "span");
            if (!string.IsNullOrEmpty(className))
                builder.Class(className);
            child?.Invoke();
            builder.CloseElement();
            return builder;
        }
    }

    public static class RenderTreeBuilderComponentExtensions
    {
        public static RenderTreeBuilder Markup(this RenderTreeBuilder builder, string markup)
        {
            if (!string.IsNullOrWhiteSpace(markup))
                builder.AddMarkupContent(1, markup);
            return builder;
        }
    }

    public static class RenderTreeBuilderElementExtensions
    {
        public static RenderTreeBuilder Class(this RenderTreeBuilder builder, string className)
        {
            if (!string.IsNullOrEmpty(className))
                builder.AddAttribute(1, "class", className);
            return builder;
        }
    }

    public static class RenderTreeBuilderParseHelper
    {
        public static RenderFragment AsContent(this string content)
        {
            return builder => builder.AddContent(0, content);
        }
    }
}
