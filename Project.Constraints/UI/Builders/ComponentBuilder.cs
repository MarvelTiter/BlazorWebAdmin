using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Project.Constraints.Common;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;

namespace Project.Constraints.UI.Builders
{
    [IgnoreAutoInject]
    public class ComponentBuilder<TComponent, TSelf> : IUIComponent
        where TComponent : IComponent
        where TSelf : ComponentBuilder<TComponent, TSelf>
    {
        public object Receiver { get; set; }

        protected readonly Dictionary<string, object?> parameters = new(StringComparer.Ordinal);
        protected Func<TSelf, RenderFragment>? newRender;
        protected Action<TSelf>? tpropHandle;
        protected readonly static ConcurrentDictionary<(Type Entity, PropertyInfo Prop), Delegate> propAssignCaches = new();

        public IUIComponent AdditionalParameters(Dictionary<string, object> parameters)
        {
            if (parameters != null)
            {
                foreach (var kv in parameters)
                {
                    if (!this.parameters.ContainsKey(kv.Key))
                    {
                        this.parameters[kv.Key] = kv.Value;
                    }
                }
            }
            return this;
        }
        public TSelf SetComponent<TProp>(Expression<Func<TComponent, TProp>> selector, TProp value)
        {
            var prop = selector.ExtractProperty();
            Set(prop.Name, value!);
            return this as TSelf;
        }
        public IUIComponent Set(string key, object value)
        {
            parameters[key] = value;
            return this;
        }

        public IUIComponent TrySet(string key, object value)
        {
            if (!parameters.ContainsKey(key))
            {
                parameters.Add(key, value);
            }
            return this;
        }

        public virtual RenderFragment Render()
        {
            return builder =>
            {
                builder.OpenComponent<TComponent>(0);
                if (parameters.Count > 0)
                    builder.AddMultipleAttributes(1, parameters!);
                builder.CloseComponent();
            };
        }
    }

    [IgnoreAutoInject]
    public class ComponentBuilder<TComponent> : ComponentBuilder<TComponent, ComponentBuilder<TComponent>>, IUIComponent
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
