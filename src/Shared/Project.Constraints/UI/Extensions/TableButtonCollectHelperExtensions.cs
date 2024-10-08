using Microsoft.AspNetCore.Components;
using Project.Constraints.UI.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Project.Constraints.UI.Extensions
{
    public static class TableButtonCollectHelperExtensions
    {
        public static List<TableButton<TModel>> CollectButtons<TModel>(this ComponentBase instance)
        {
            List<TableButton<TModel>> buttons = new List<TableButton<TModel>>();
            var type = instance.GetType();
            var methods = type.GetMethods().Where(m => m.GetCustomAttribute<TableButtonAttribute>() != null);
            foreach (var method in methods)
            {
                var btnOptions = method.GetCustomAttribute<TableButtonAttribute>()!;
                ArgumentNullException.ThrowIfNull(btnOptions.Label ?? btnOptions.LabelExpression);
                var btn = new TableButton<TModel>(btnOptions);
                btn.Callback = method.CreateDelegate<Func<TModel, Task<IQueryResult>>>(instance);
                if (btnOptions.LabelExpression != null)
                {
                    var le = type.GetMethod(btnOptions.LabelExpression);
                    btn.LabelExpression = le?.CreateDelegate<Func<TableButtonContext<TModel>, string>>(instance);
                }

                if (btnOptions.VisibleExpression != null)
                {
                    var ve = type.GetMethod(btnOptions.VisibleExpression);
                    btn.VisibleExpression = ve?.CreateDelegate<Func<TableButtonContext<TModel>, bool>>(instance);
                }
                buttons.Add(btn);
            }
            return buttons;
        }
    }
}
