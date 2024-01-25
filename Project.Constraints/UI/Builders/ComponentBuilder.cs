using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Project.Constraints.Common;
using System.Linq.Expressions;

namespace Project.Constraints.UI.Builders
{

    [IgnoreAutoInject]
    public class ComponentBuilder<TComponent> : ComponentBuilderBasic<TComponent, ComponentBuilder<TComponent>>, IUIComponent
        where TComponent : IComponent
    {

    }
    [IgnoreAutoInject]
    public class CustomComponentBuilder<TComponent> where TComponent : IComponent
    {
        private readonly RenderTreeBuilder builder;

        public CustomComponentBuilder(RenderTreeBuilder builder)
        {
            this.builder = builder;
        }
        protected readonly Dictionary<string, object?> parameters = new(StringComparer.Ordinal);

        public CustomComponentBuilder<TComponent> SetComponent<TProp>(Expression<Func<TComponent, TProp>> selector, TProp value)
        {
            var prop = selector.ExtractProperty();
            return Set(prop.Name, value!);
        }

        private CustomComponentBuilder<TComponent> Set(string key, object value)
        {
            parameters[key] = value;
            return this;
        }

        public void Build(Action<object>? capture = null)
        {
            builder.OpenComponent<TComponent>(0);
            if (parameters.Count > 0)
            {
                builder.AddMultipleAttributes(1, parameters!);
            }
            if (capture != null)
            {
                builder.AddComponentReferenceCapture(2, capture);
            }
            builder.CloseComponent();
        }
    }

    
}
