﻿using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Project.Common.Attributes;
using Project.Constraints.UI;
using Project.Constraints.UI.Table;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;
using static Project.Web.Shared.Components.InputBuilderHelper;
namespace Project.Web.Shared.Components
{
    public class InputBuilderHelper
    {
        public static Expression<Func<TReturn>> BinderExpressionBuilder<TReturn>(Expression body)
        {
            return Expression.Lambda<Func<TReturn>>(body);
        }

        

        public static InputType GetInputType(Type type, ColumnInfo col)
        {
            if (col.InputType.HasValue)
                return col.InputType.Value;
            type = Nullable.GetUnderlyingType(type) ?? type;
            if (type.IsEnum)
            {
                return InputType.Select;
            }
            return Type.GetTypeCode(type) switch
            {
                TypeCode.UInt16 or TypeCode.UInt32 or TypeCode.UInt64 or TypeCode.Int16 or TypeCode.Int32 or TypeCode.Int64 or TypeCode.Decimal or TypeCode.Double or TypeCode.Single => InputType.Number,
                TypeCode.Boolean => InputType.Boolean,
                TypeCode.DateTime => InputType.DatePicker,
                _ => InputType.Text,
            };
        }
    }
    public class InputBuilder<TData> : ComponentBase
    {
        [Parameter] public ColumnInfo Column { get; set; }
        [Parameter] public IUIService UI { get; set; }
        [Parameter] public object Reciver { get; set; }
        [Parameter] public TData Data { get; set; }
        [CascadingParameter] public bool Edit { get; set; }

        private string TempValue { get; set; }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            var instance = Expression.Constant(Data);
            var propExp = Expression.Property(instance, Column.Property);
            var fragment = GetInputType(Column, propExp);
            fragment.Set("disabled", Edit && Column.Readonly).Render().Invoke(builder);
        }


        public static MethodInfo builder = typeof(InputBuilderHelper).GetMethod(nameof(BinderExpressionBuilder))!;

        static readonly ConcurrentDictionary<ColumnInfo, Func<IUIService, object, Expression, IUIComponent>> builderCaches = new();

        public IUIComponent GetInputType(ColumnInfo column, Expression propertyExpression)
        {
            if (column.IsEnum || column.EnumValues != null)
            {
                return UI.BuildSelect<KeyValuePair<string, string>, string>(Reciver, Column.EnumValues!).LabelExpression(kv => kv.Value).ValueExpression(kv => kv.Key).Bind(() => TempValue, () => UpdateValue(column.Property));
            }
            var func = builderCaches.GetOrAdd(column, col =>
              {
                  var parameterUI = Expression.Parameter(typeof(IUIService));
                  var parameterRe = Expression.Parameter(typeof(object));
                  var parameterEx = Expression.Parameter(typeof(Expression));
                  MethodInfo? builderMethod = null;
                  var propertyType = col.DataType;


                  switch (InputBuilderHelper.GetInputType(propertyType, col))
                  {
                      case InputType.Text:
                          builderMethod = typeof(IUIService).GetMethod(nameof(IUIService.BuildInput), 0, [typeof(object)]);
                          break;
                      case InputType.Number:
                          builderMethod = typeof(IUIService).GetMethod(nameof(IUIService.BuildInput), 1, [typeof(object)])?.MakeGenericMethod(propertyType);
                          break;
                      case InputType.Boolean:
                          builderMethod = typeof(IUIService).GetMethod(nameof(IUIService.BuildSwitch));
                          break;
                      case InputType.DatePicker:
                          builderMethod = typeof(IUIService).GetMethod(nameof(IUIService.BuildDatePicker))?.MakeGenericMethod(propertyType);
                          break;
                      case InputType.Password:
                          builderMethod = typeof(IUIService).GetMethod(nameof(IUIService.BuildPassword));
                          break;
                  }
                  if (builderMethod == null)
                  {
                      throw new ArgumentNullException(nameof(builderMethod));
                  }
                  /*
                   * var expression = Expression.Lambda<Func<TReturn>>(propertyExpression);
                   * (IUIComponent)ui.BuildXXX<TReturn>(reciver).Bind((Expression<Func<TReturn>>)expression)
                   */

                  var expType = typeof(Expression<>).MakeGenericType(typeof(Func<>).MakeGenericType(propertyType));
                  var binder = Expression.Call(parameterUI, builderMethod, parameterRe);
                  var funcExp = Expression.Call(null, builder.MakeGenericMethod(propertyType), parameterEx);
                  var body = Expression.Call(binder, "Bind", null, Expression.Convert(funcExp, expType));
                  return Expression.Lambda<Func<IUIService, object, Expression, IUIComponent>>(Expression.Convert(body, typeof(IUIComponent)), parameterUI, parameterRe, parameterEx).Compile();
              });

            return func.Invoke(UI, Reciver, propertyExpression);
        }

        private Task UpdateValue(PropertyInfo property)
        {
            property.SetValue(Data,ConvertTo(property.PropertyType, TempValue));
            return Task.CompletedTask;
        }


        public static object ConvertTo(Type type, object value, object defaultValue = null)
        {
            if (value == null || value == DBNull.Value)
                return defaultValue;

            var valueString = value.ToString()!;
            if (type == typeof(string))
                return Convert.ChangeType(valueString, type);

            valueString = valueString.Trim();
            if (valueString.Length == 0)
                return defaultValue;

            if (type.IsEnum)
                return Enum.Parse(type, valueString, true);

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                type = Nullable.GetUnderlyingType(type)!;

            if (type == typeof(bool) || type == typeof(bool?))
                valueString = ",是,1,Y,YES,TRUE,".Contains(valueString.ToUpper()) ? "True" : "False";

            try
            {
                return Convert.ChangeType(valueString, type);
            }
            catch
            {
                return defaultValue;
            }
        }
    }
}
