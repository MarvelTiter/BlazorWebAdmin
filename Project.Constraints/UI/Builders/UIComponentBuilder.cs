using Microsoft.AspNetCore.Components;

namespace Project.Constraints.UI.Builders
{
    [IgnoreAutoInject]
    public class UIComponentBuilder<TComponent, TPropModel> : InputComponentBuilder<TComponent, TPropModel, UIComponentBuilder<TComponent, TPropModel>>
        where TComponent : IComponent
        where TPropModel : new()
    {
        public UIComponentBuilder()
        {
            
        }
        public UIComponentBuilder(Action<UIComponentBuilder<TComponent, TPropModel>> action)
        {
            this.tpropHandle = action;
        }
    }
}
