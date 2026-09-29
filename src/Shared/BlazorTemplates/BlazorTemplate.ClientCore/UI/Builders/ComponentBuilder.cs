using Microsoft.AspNetCore.Components.Rendering;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace BlazorTemplate.ClientCore.UI.Builders;

public class ComponentBuilder<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TComponent> : ComponentBuilderBasic<TComponent, ComponentBuilder<TComponent>>, IUIComponent
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
public class CustomComponentBuilder<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TComponent>(RenderTreeBuilder builder)
    where TComponent : IComponent
{
    protected readonly Dictionary<string, object?> parameters = new(StringComparer.Ordinal);

    public CustomComponentBuilder<TComponent> SetComponent<TProp>(Expression<Func<TComponent, TProp>> selector, TProp value)
    {
        var key = selector.ExtractPropertyName();
        return Set(key, value!);
    }

    public CustomComponentBuilder<TComponent> SetContent(RenderFragment content)
    {
        return Set("ChildContent", content);
    }

    public CustomComponentBuilder<TComponent> AdditionalParameters(Dictionary<string, object?> parameters)
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
        if (capture is not null)
        {
            builder.AddComponentReferenceCapture(2, capture);
        }
        builder.CloseComponent();
    }
}