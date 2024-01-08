using Microsoft.AspNetCore.Components;
using Project.Common.Attributes;
using Project.Constraints.UI.Props;

namespace Project.Constraints.UI.Builders
{
    [IgnoreAutoInject]
    public class SelectComponentBuilder<TComponent, TPropModel, TItem, TValue> : BindableInputComponentBuilder<TComponent, TPropModel, TValue, SelectComponentBuilder<TComponent, TPropModel, TItem, TValue>>, IBindableInputComponent<TPropModel, TValue>, ISelectInput<TPropModel, TItem, TValue>
        where TComponent : IComponent
        where TPropModel : DefaultProp, new()
    {
        public SelectComponentBuilder()
        {

        }

        public SelectComponentBuilder(Func<SelectComponentBuilder<TComponent, TPropModel, TItem, TValue>, RenderFragment> newRender)
        {
            this.newRender = newRender;
        }

        public SelectComponentBuilder(Action<SelectComponentBuilder<TComponent, TPropModel, TItem, TValue>> tpropHandle)
        {
            this.tpropHandle = tpropHandle;
        }
    }
}
