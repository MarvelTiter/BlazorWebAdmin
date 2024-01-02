using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Project.Constraints.UI;
using Project.Constraints.UI.Table;
using System.Linq.Expressions;

namespace Project.Web.Shared.Components
{
    public class InputBuilder<TData> : ComponentBase
    {
        [Parameter] public ColumnInfo Column { get; set; }
        [Parameter] public IUIService UI { get; set; }
        [Parameter] public object Reciver { get; set; }
        [Parameter] public TData Data { get; set; }
        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            var instance = Expression.Constant(Data);
            var propExp = Expression.Property(instance, Column.Property);
            var fragment = GetInputType(Column, propExp);
            fragment.Render().Invoke(builder);
        }

        public IUIComponent GetInputType(ColumnInfo column, Expression propertyExpression)
        {
            var type = column.DataType;

            if (type == typeof(bool))
            {
                var binder = Expression.Lambda<Func<bool>>(propertyExpression);
                return UI.BuildSwitch(Reciver).Bind(binder);
            }

            if (type == typeof(short))
            {
                var binder = Expression.Lambda<Func<short>>(propertyExpression);
                return UI.BuildInput<short>(Reciver).Bind(binder);
            }

            if (type == typeof(int))
            {
                var binder = Expression.Lambda<Func<int>>(propertyExpression);
                return UI.BuildInput<int>(Reciver).Bind(binder);
            }
            if (type == typeof(int?))
            {
                var binder = Expression.Lambda<Func<int?>>(propertyExpression);
                return UI.BuildInput<int?>(Reciver).Bind(binder);
            }

            if (type == typeof(long))
            {
                var binder = Expression.Lambda<Func<long>>(propertyExpression);
                return UI.BuildInput<long>(Reciver).Bind(binder);
            }

            if (type == typeof(float))
            {
                var binder = Expression.Lambda<Func<float>>(propertyExpression);
                return UI.BuildInput<float>(Reciver).Bind(binder);
            }

            if (type == typeof(double))
            {
                var binder = Expression.Lambda<Func<double>>(propertyExpression);
                return UI.BuildInput<double>(Reciver).Bind(binder);
            }

            if (type == typeof(decimal))
            {
                var binder = Expression.Lambda<Func<decimal>>(propertyExpression);
                return UI.BuildInput<decimal>(Reciver).Bind(binder);
            }

            if (type == typeof(DateTime?))
            {
                var binder = Expression.Lambda<Func<DateTime?>>(propertyExpression);
                return UI.BuildDatePicker<DateTime?>(Reciver).Bind(binder);
            }

            if (type == typeof(DateTime))
            {
                var binder = Expression.Lambda<Func<DateTime>>(propertyExpression);
                return UI.BuildDatePicker<DateTime>(Reciver).Bind(binder);
            }

            if (type == typeof(DateTimeOffset?))
            {
                var binder = Expression.Lambda<Func<DateTimeOffset?>>(propertyExpression);
                return UI.BuildDatePicker<DateTimeOffset?>(Reciver).Bind(binder);
            }

            if (type == typeof(DateTimeOffset))
            {
                var binder = Expression.Lambda<Func<DateTimeOffset>>(propertyExpression);
                return UI.BuildDatePicker<DateTimeOffset>(Reciver).Bind(binder);
            }

            //if (column.IsEnum || column.EnumValues != null)
            //{
            //    if (column.EnumValues != null)
            //        return UI.BuildSelect<KeyValuePair<string, string>, string>(Reciver, column.EnumValues).ValueExpression(k=>k.Key).LabelExpression(k=>k.Value).Bind();
            //}

            //if (type == typeof(string[]) || column.Type == FieldType.CheckList)
            //    return typeof(AntCheckboxGroup);

            //if (column.Type == FieldType.RadioList)
            //    return typeof(AntRadioGroup);

            //if (column.Type == FieldType.Password)
            //    return typeof(InputPassword);

            //if (column.Type == FieldType.TextArea)
            //    return typeof(TextArea);

            var binder2 = Expression.Lambda<Func<string>>(propertyExpression);


            return UI.BuildInput(Reciver).Bind(binder2);
        }
    }
}
