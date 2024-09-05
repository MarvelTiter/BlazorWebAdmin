using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Project.Constraints.Common;
using Project.Constraints.UI.Extensions;
using Project.Constraints.UI.Props;
using System.Linq.Expressions;
using System.Reflection;

namespace Project.Constraints.UI.Builders
{
    public class ElementBuilder
    {
        private readonly RenderTreeBuilder builder;
        private readonly string elementName;

        public ElementBuilder(RenderTreeBuilder builder, string elementName)
        {
            this.builder = builder;
            this.elementName = elementName;
        }

        protected readonly Dictionary<string, object?> parameters = new(StringComparer.Ordinal);

        public ElementBuilder Set<TProp>(Expression<Func<HtmlProp, TProp>> selector, TProp value)
        {
            var prop = selector.ExtractProperty();
            var attr = prop.GetCustomAttribute<PropNameAttribute>();
            var name = attr?.Name ?? prop.Name;
            return Set(name, value!);
        }
        RenderFragment? content;
        public ElementBuilder AddContent(RenderFragment content)
        {
            this.content = content;
            return this;
        }
        public ElementBuilder AddText(string? text)
        {
            AddContent(text.AsContent());
            return this;
        }

        public ElementBuilder Set(string key, object value)
        {
            parameters[key] = value;
            return this;
        }

        public void Build(Action<ElementReference>? capture = null)
        {
            builder.OpenElement(0, elementName);
            if (parameters.Count > 0)
            {
                builder.AddMultipleAttributes(1, parameters!);
            }
            if (capture != null)
            {
                builder.AddElementReferenceCapture(2, capture);
            }
            if (content != null)
            {
                builder.AddContent(3, content);
            }
            builder.CloseElement();
        }

    }

    
}
