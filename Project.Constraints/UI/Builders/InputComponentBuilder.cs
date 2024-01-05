using Microsoft.AspNetCore.Components;
using Project.Common;
using Project.Common.Attributes;
using System.Linq.Expressions;

namespace Project.Constraints.UI.Builders
{
    [IgnoreAutoInject]
    public class InputComponentBuilder<TComponent, TPropModel, TSelf> : ComponentBuilder<TComponent, TSelf>, IInputComponent<TPropModel>
        where TPropModel : new()
        where TComponent : IComponent
        where TSelf : ComponentBuilder<TComponent, TSelf>

    {
        public TPropModel Model { get; set; } = new();

        public IInputComponent<TPropModel> Set<TMember>(Expression<Func<TPropModel, TMember>> selector, TMember value)
        {
            /**
            * v => model.XXX = v;
            */
            var prop = selector.ExtractProperty();
            var modelExp = Expression.Constant(Model);
            var p = Expression.Parameter(typeof(TMember));
            var action = Expression.Lambda<Action<TMember>>(Expression.Assign(Expression.Property(modelExp, prop), p), p);
            action.Compile().Invoke(value);
            return this;
        }

        public override RenderFragment Render()
        {
            tpropHandle?.Invoke((this as TSelf)!);
            return newRender?.Invoke((this as TSelf)!) ?? base.Render();
        }
    }
}
