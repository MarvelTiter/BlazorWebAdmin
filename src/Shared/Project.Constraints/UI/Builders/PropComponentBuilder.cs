using Microsoft.AspNetCore.Components;
using Project.Constraints.Common;
using System.Linq.Expressions;

namespace Project.Constraints.UI.Builders
{
    public class PropComponentBuilder<TComponent, TPropModel> : ComponentBuilderBasic<TComponent, PropComponentBuilder<TComponent, TPropModel>>, IUIComponent<TPropModel>
        where TPropModel : new()
        where TComponent : IComponent
    {
        public TPropModel Model { get; set; } = new();

        public PropComponentBuilder()
        {

        }

        public PropComponentBuilder(Func<PropComponentBuilder<TComponent, TPropModel>, RenderFragment> newRender)
        {
            this.newRender = newRender;
        }

        public PropComponentBuilder(Action<PropComponentBuilder<TComponent, TPropModel>> tpropHandle)
        {
            this.tpropHandle = tpropHandle;
        }

        public IUIComponent<TPropModel> Set<TMember>(Expression<Func<TPropModel, TMember>> selector, TMember value)
        {
            /**
            * v => model.XXX = v;
            */
            //var prop = selector.ExtractProperty();
            //var modelExp = Expression.Constant(Model);
            //var p = Expression.Parameter(typeof(TMember));
            //var action = Expression.Lambda<Action<TMember>>(Expression.Assign(Expression.Property(modelExp, prop), p), p);
            //action.Compile().Invoke(value);
            //return this;
            var prop = selector.ExtractProperty();
            var action = propAssignCaches.GetOrAdd((typeof(TPropModel), prop), static key =>
            {
                var modelExp = Expression.Parameter(key.Entity);
                var p = Expression.Parameter(key.Prop.PropertyType);
                return Expression.Lambda<Action<TPropModel, TMember>>(Expression.Assign(Expression.Property(modelExp, key.Prop), p), modelExp, p).Compile();
            });
            action.DynamicInvoke(Model, value);
            return this;
        }

        public override RenderFragment Render()
        {
            tpropHandle?.Invoke(this);
            return newRender?.Invoke(this) ?? base.Render();
        }
    }
}
