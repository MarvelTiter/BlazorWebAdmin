using BlazorWeb.Shared.Template.Tables.Setting;
using Microsoft.AspNetCore.Components;
using MT.Toolkit.DateTimeExtension;
using MT.Toolkit.StringExtension;
using System.Linq.Expressions;

namespace BlazorWeb.Shared.Components
{
    public interface ICondition
    {

    }
    public enum DateType
    {
        DayStart,
        DayEnd,
    }

    public partial class Condition : ConditionBase
    {
        //[CascadingParameter] public IQueryCondition Parent { get; set; }
        //[Parameter] public string? Label { get; set; }
        [Parameter] public CompareType Compare { get; set; } = CompareType.Equal;
        //[Parameter] public DateType? DateConfig { get; set; }
        [Parameter] public TableOptionColumn? Field { get; set; }
        [Parameter] public RenderFragment ChildContent { get; set; }
        [Parameter] public object? DefaultValue { get; set; }
        [Inject] public ILogger<Condition> Logger { get; set; }
        public int Index { get; set; }
        DateTime dateValue = DateTime.Now;
        string stringValue = string.Empty;

        protected override void OnInitialized()
        {
            base.OnInitialized();
            Index = Parent.Conditions.Count;
            Parent.AddCondition(this);
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
                }
                await NotifyChanged();
            }
        }

        object GetInnerValue(out bool legal)
        {
            if (Field == null)
            {
                legal = false;
                return null;
            }
            if (Field.DataType == typeof(DateTime))
            {
                legal = dateValue != default;
                if (DateConfig.HasValue)
                {
                    return DateConfig.Value == DateType.DayStart ? dateValue.DayStart() : dateValue.DayEnd();
                }
                return dateValue;
            }
            else if (Field.EnumValues != null && Field.IsEnum)
            {
                legal = stringValue.IsEnable();
                return stringValue;
            }
            else
            {
                legal = stringValue.IsEnable();
                if (legal && IsNumberOrDateTime())
                {
                    legal = stringValue.IsNumeric<int>(out _);
                }
                return stringValue;
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
            var innerValue = GetInnerValue(out bool validValue);
            //if (validValue)
            //    Logger.LogInformation(innerValue.ToString());
            var condition = new ConditionInfo(Field.PropertyOrFieldName, Compare, innerValue, Field.DataType, validValue);
            condition.LinkType = Index > 0 ? ExpressionType.AndAlso : null;
            return Parent.UpdateCondition(Index, condition);
        }
    }
}
