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
    }
}
