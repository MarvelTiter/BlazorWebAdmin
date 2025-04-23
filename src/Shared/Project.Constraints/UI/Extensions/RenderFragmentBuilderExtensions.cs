using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Project.Constraints.UI.Builders;

namespace Project.Constraints.UI.Extensions
{
    public static class RenderFragmentBuilderExtensions
    {
        public static CustomComponentBuilder<TComponent> Component<TComponent>(this RenderTreeBuilder builder) where TComponent : IComponent
        {
            return new CustomComponentBuilder<TComponent>(builder);
        }

        public static ElementBuilder Div(this RenderTreeBuilder builder)
        {
            return new ElementBuilder(builder, "div");
        }

        public static ElementBuilder Div(this RenderTreeBuilder builder, RenderFragment content)
        {
            return new ElementBuilder(builder, "div")
                .AddContent(content);
        }

        public static ElementBuilder Span(this RenderTreeBuilder builder)
        {
            return new ElementBuilder(builder, "span");
        }
        
        public static ElementBuilder Span(this RenderTreeBuilder builder, string content)
        {
            return builder.Span().AddText(content);
        }

        public static ElementBuilder P(this RenderTreeBuilder builder)
        {
            return new ElementBuilder(builder, "p");
        }

        public static RenderFragment AsContent(this string? content)
        {
            return builder => builder.AddContent(1, content);
        }

        public static RenderFragment AsMarkupString(this string content)
        {
            return builder => builder.AddContent(1, new MarkupString(content));
        }
    }
}