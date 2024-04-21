using Microsoft.AspNetCore.Components;
using Project.Constraints.UI.Props;

namespace Project.Constraints.UI.Builders
{
    [IgnoreAutoInject]
    public class SelectComponentBuilder<TComponent, TPropModel, TItem, TValue> : BindableComponentBuilder<TComponent, TPropModel, TValue>, IBindableInputComponent<TPropModel, TValue>, ISelectInput<TPropModel, TItem, TValue>
        where TComponent : IComponent
        where TPropModel : DefaultProp, new()
    {
        public SelectComponentBuilder()
        {

        }

        public SelectComponentBuilder(Func<PropComponentBuilder<TComponent, TPropModel>, RenderFragment> newRender)
        {
            this.newRender = newRender;
        }

        public SelectComponentBuilder(Action<PropComponentBuilder<TComponent, TPropModel>> tpropHandle)
        {
            this.tpropHandle = tpropHandle;
        }
    }
}
