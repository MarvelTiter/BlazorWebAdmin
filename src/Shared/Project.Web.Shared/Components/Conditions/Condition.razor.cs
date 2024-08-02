using Microsoft.AspNetCore.Components;
using MT.Toolkit.DateTimeExtension;
using MT.Toolkit.StringExtension;
using System.Linq.Expressions;
using Project.Constraints.UI.Table;
using Project.Constraints.UI;
using Microsoft.Extensions.Logging;

namespace Project.Web.Shared.Components
{
    public interface ICondition
    {

    }
    public enum DateType
    {
        DayStart,
        DayEnd,
        /// <summary>
        /// 当天时间
        /// </summary>
        InDay,
    }

    public partial class Condition : ConditionBase
    {
        [Parameter] public CompareType Compare { get; set; } = CompareType.Equal;
        [Parameter] public ColumnInfo? Field { get; set; }
        [Parameter] public object? DefaultValue { get; set; }
        public int Index { get; set; }
        DateTime dateValue = DateTime.Now;
        string stringValue = string.Empty;

        protected override void OnInitialized()
        {
            base.OnInitialized();
            Index = Parent!.Conditions.Count + Parent.IndexFixed;
            if (DateConfig.HasValue && DateConfig.Value == DateType.InDay)
            {
                Parent.IndexFixed += 1;
            }
            Parent.AddCondition(this);
            //Parent.RowBuilder.AddCol(ActualColWidth).AddContent(CreateBody());
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
            if (firstRender)
            {
                if (DefaultValue != null)
                {
                    if (DefaultValue is DateTime d) dateValue = d;
                    else stringValue = $"{DefaultValue}";
                    await InvokeAsync(StateHasChanged);
                }
                await NotifyChanged();
            }
        }

        static (object?, CompareType)[] CreateTuple(params (object?, CompareType)[] values) => values;

        (object?, CompareType)[] GetInnerValue(out bool legal)
        {
            if (Field == null)
            {
                legal = false;
                return CreateTuple((null, Compare));
            }
            if (Field.DataType == typeof(DateTime))
            {
                legal = dateValue != default;
                if (DateConfig.HasValue)
                {
                    if (DateConfig.Value == DateType.InDay)
                    {
                        return CreateTuple((dateValue.DayStart(), CompareType.GreaterThanOrEqual), (dateValue.DayEnd(), CompareType.LessThan));
                    }
                    else
                    {
                        var d = DateConfig.Value == DateType.DayStart ? dateValue.DayStart() : dateValue.DayEnd();
                        return CreateTuple((d, Compare));
                    }
                }
                return CreateTuple((dateValue, Compare));
            }
            else if (Field.EnumValues != null && Field.IsEnum)
            {
                legal = stringValue.IsEnable();
                return CreateTuple((stringValue, Compare));

            }
            else
            {
                legal = stringValue.IsEnable();
                if (legal && IsNumberOrDateTime())
                {
                    legal = stringValue.IsNumeric<int>(out _);
                }
                return CreateTuple((stringValue, Compare));
            }

            bool IsNumberOrDateTime()
            {
                return Field.DataType == typeof(Int16)
                || Field.DataType == typeof(Int32)
                || Field.DataType == typeof(Int64)
                || Field.DataType == typeof(Single)
                || Field.DataType == typeof(Double)
                || Field.DataType == typeof(DateTime)
                || Field.DataType == typeof(Int16?)
                || Field.DataType == typeof(Int32?)
                || Field.DataType == typeof(Int64?)
                || Field.DataType == typeof(Single?)
                || Field.DataType == typeof(Double?)
                || Field.DataType == typeof(DateTime?);
            }
        }


        Task NotifyChanged()
        {
            if (Field == null) return Task.CompletedTask;
            var values = GetInnerValue(out bool validValue);
            int count = 0;
            foreach (var value in values)
            {
                if (Parent!.TryGetCondition(Index + count, out var c))
                {
                    c!.Value = validValue;
                }
                else
                {
                    c = new ConditionUnit();
                    c.Name = Field.PropertyOrFieldName;
                    c.Value = value.Item1;
                    c.CompareType = value.Item2;
                    Parent!.UpdateCondition(Index + count, c);
                }
                //var condition = new ConditionInfo(Field.PropertyOrFieldName, value.Item2, value.Item1, Field.DataType, validValue);
                //condition.LinkType = Index > 0 || count > 0 ? ExpressionType.AndAlso : null;
                //await Parent!.UpdateCondition(Index + count, condition);
                count++;
            }
            return Task.CompletedTask;
        }
    }
}
