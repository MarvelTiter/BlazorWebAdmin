using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Project.Constraints.Common;
using System.Linq.Expressions;

namespace Project.Constraints.UI.Builders;

public class ComponentBuilder<TComponent> : ComponentBuilderBasic<TComponent, ComponentBuilder<TComponent>>, IUIComponent
    where TComponent : IComponent
{
    public ComponentBuilder()
    {
            
    }

    public ComponentBuilder(Action<ComponentBuilder<TComponent>> action)
    {
        this.tpropHandle = action;
    }

    public ComponentBuilder(Func<ComponentBuilder<TComponent>, RenderFragment> func)
    {
        this.newRender = func;
    }
}
public class CustomComponentBuilder<TComponent>(RenderTreeBuilder builder)
    where TComponent : IComponent
{
    protected readonly Dictionary<string, object?> parameters = new(StringComparer.Ordinal);

    public CustomComponentBuilder<TComponent> SetComponent<TProp>(Expression<Func<TComponent, TProp>> selector, TProp value)
    {
        var prop = selector.ExtractProperty();
        return Set(prop.Name, value!);
    }

    public CustomComponentBuilder<TComponent> SetContent(RenderFragment content)
    {
        return Set("ChildContent", content);
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