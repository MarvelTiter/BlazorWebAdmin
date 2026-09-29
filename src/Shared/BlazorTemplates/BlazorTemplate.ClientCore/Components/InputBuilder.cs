using Microsoft.AspNetCore.Components.Rendering;
using BlazorTemplate.ClientCore.UI;
using BlazorTemplate.ClientCore.UI.Extensions;
using System.Linq.Expressions;
using BlazorTemplate.ClientCore.Lookup;

namespace BlazorTemplate.ClientCore.Components;

public class InputBuilderHelper
{
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
    [Parameter, NotNull] public ColumnInfo? Column { get; set; }
    [Parameter, NotNull] public IUIService? UI { get; set; }
    [Parameter, NotNull] public object? Reciver { get; set; }
    [Parameter, NotNull] public TData? Data { get; set; }
    [Parameter, NotNull] public ILookupService? Lookup { get; set; }
    [CascadingParameter] public bool Edit { get; set; }

    private string TempValue { get; set; } = string.Empty;

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (Column.FormTemplate != null)
        {
            Column.FormTemplate.Invoke(new ColumnItemContext(Data, Column)).Invoke(builder);
            return;
        }
        var instance = Expression.Constant(Data);
        var propExp = Expression.Property(instance, Column.PropertyOrFieldName);
        var fragment = GetInputType(Column, propExp);
        if (Edit && Column.Readonly)
            fragment.Set("disabled", Edit && Column.Readonly);
        fragment.Render().Invoke(builder);
    }

    /// <summary>
    ///     依据列信息选择并绑定输入组件。
    ///     <para>
    ///         实现上按 <see cref="ColumnInfo.DataType" /> 做<strong>编译期封闭</strong>的泛型分发，
    ///         不再用 <c>MethodInfo.MakeGenericMethod</c> + 表达式树 <c>Compile()</c> 组装绑定器。
    ///     </para>
    ///     <para>
    ///         原因是 <c>ColumnInfo.DataType</c> 只有运行时才确定，而运行时的泛型实例化在 AOT 下要求
    ///         实参在编译期已知（值类型尤其如此），原写法属于无法靠标注规避的硬阻塞。改成封闭分发后
    ///         泛型实参全部是字面量，AOT / 裁剪下不产生任何动态代码生成，也不需要再按列缓存编译委托。
    ///     </para>
    ///     <para>
    ///         泛型实参必须与属性声明类型<strong>完全一致</strong>（含 Nullable 包裹），
    ///         否则 <c>Bind</c> 的表达式类型与组件期望的类型对不上。
    ///     </para>
    /// </summary>
    public IUIComponent GetInputType(ColumnInfo column, Expression propertyExpression)
    {
        if (column.LookupType is { })
        {
            TempValue = column.GetValue(Data)?.ToString() ?? string.Empty;
            return UI.BuildSelect<LookupEntry, string>(Reciver, Lookup.GetEntries(column.LookupType))
                .LabelExpression(kv => kv.DisplayName)
                .ValueExpression(kv => kv.Code)
                .Bind(() => TempValue, () => UpdateValue(column));
        }

        var type = column.DataType;
        var inputType = InputBuilderHelper.GetInputType(type, column);

        return inputType switch
        {
            // —— 文本 / 密码：组件本身是 string 类型，只接受 string 属性
            InputType.Text when type == typeof(string) => UI.BuildInput(Reciver).Bind(Make<string>(propertyExpression)),
            InputType.Password when type == typeof(string) => UI.BuildPassword(Reciver).Bind(Make<string>(propertyExpression)),
            // —— 开关：IUIService.BuildSwitch 固定为 bool，没有 bool? 重载，
            //    因此可空 bool 属性无法绑定（旧实现走 Expression.Convert 的参考转换，
            //    编译期通过、运行时 InvalidCastException），这里落到末尾明确报错。
            InputType.Boolean when type == typeof(bool) => UI.BuildSwitch(Reciver).Bind(Make<bool>(propertyExpression)),
            // —— 日期
            InputType.DatePicker when type == typeof(DateTime) => UI.BuildDatePicker<DateTime>(Reciver).Bind(Make<DateTime>(propertyExpression)),
            InputType.DatePicker when type == typeof(DateTime?) => UI.BuildDatePicker<DateTime?>(Reciver).Bind(Make<DateTime?>(propertyExpression)),
            // —— 数值：可空与非可空各一个封闭实例（Nullable<T> 同样满足 BuildNumberInput 的 new() 约束）
            InputType.Number when type == typeof(byte) => UI.BuildNumberInput<byte>(Reciver).Bind(Make<byte>(propertyExpression)),
            InputType.Number when type == typeof(byte?) => UI.BuildNumberInput<byte?>(Reciver).Bind(Make<byte?>(propertyExpression)),
            InputType.Number when type == typeof(sbyte) => UI.BuildNumberInput<sbyte>(Reciver).Bind(Make<sbyte>(propertyExpression)),
            InputType.Number when type == typeof(sbyte?) => UI.BuildNumberInput<sbyte?>(Reciver).Bind(Make<sbyte?>(propertyExpression)),
            InputType.Number when type == typeof(short) => UI.BuildNumberInput<short>(Reciver).Bind(Make<short>(propertyExpression)),
            InputType.Number when type == typeof(short?) => UI.BuildNumberInput<short?>(Reciver).Bind(Make<short?>(propertyExpression)),
            InputType.Number when type == typeof(ushort) => UI.BuildNumberInput<ushort>(Reciver).Bind(Make<ushort>(propertyExpression)),
            InputType.Number when type == typeof(ushort?) => UI.BuildNumberInput<ushort?>(Reciver).Bind(Make<ushort?>(propertyExpression)),
            InputType.Number when type == typeof(int) => UI.BuildNumberInput<int>(Reciver).Bind(Make<int>(propertyExpression)),
            InputType.Number when type == typeof(int?) => UI.BuildNumberInput<int?>(Reciver).Bind(Make<int?>(propertyExpression)),
            InputType.Number when type == typeof(uint) => UI.BuildNumberInput<uint>(Reciver).Bind(Make<uint>(propertyExpression)),
            InputType.Number when type == typeof(uint?) => UI.BuildNumberInput<uint?>(Reciver).Bind(Make<uint?>(propertyExpression)),
            InputType.Number when type == typeof(long) => UI.BuildNumberInput<long>(Reciver).Bind(Make<long>(propertyExpression)),
            InputType.Number when type == typeof(long?) => UI.BuildNumberInput<long?>(Reciver).Bind(Make<long?>(propertyExpression)),
            InputType.Number when type == typeof(ulong) => UI.BuildNumberInput<ulong>(Reciver).Bind(Make<ulong>(propertyExpression)),
            InputType.Number when type == typeof(ulong?) => UI.BuildNumberInput<ulong?>(Reciver).Bind(Make<ulong?>(propertyExpression)),
            InputType.Number when type == typeof(float) => UI.BuildNumberInput<float>(Reciver).Bind(Make<float>(propertyExpression)),
            InputType.Number when type == typeof(float?) => UI.BuildNumberInput<float?>(Reciver).Bind(Make<float?>(propertyExpression)),
            InputType.Number when type == typeof(double) => UI.BuildNumberInput<double>(Reciver).Bind(Make<double>(propertyExpression)),
            InputType.Number when type == typeof(double?) => UI.BuildNumberInput<double?>(Reciver).Bind(Make<double?>(propertyExpression)),
            InputType.Number when type == typeof(decimal) => UI.BuildNumberInput<decimal>(Reciver).Bind(Make<decimal>(propertyExpression)),
            InputType.Number when type == typeof(decimal?) => UI.BuildNumberInput<decimal?>(Reciver).Bind(Make<decimal?>(propertyExpression)),
            // 枚举列在上面已由 LookupType 分支接走；走到这里说明列声明的 InputType 与属性类型对不上，
            // 原实现在这种组合下同样无法工作（会抛 ArgumentException / InvalidCastException），此处给出明确原因。
            _ => throw new NotSupportedException(
                        $"无法为列 \"{column.PropertyOrFieldName}\"（属性类型 {type}）构建 {inputType} 类型的表单输入组件。"),
        };
    }

    /// <summary>
    ///     把属性访问表达式包装成 <c>Bind</c> 需要的无参取值表达式。
    ///     使用 <see cref="Expression.Lambda{TDelegate}(Expression, ParameterExpression[])" /> 泛型重载，
    ///     它不接受动态生成的委托类型，因此不带 RequiresDynamicCode 标注。
    /// </summary>
    private static Expression<Func<T>> Make<T>(Expression body) => Expression.Lambda<Func<T>>(body);

    private Task UpdateValue(ColumnInfo col)
    {
        //property.SetValue(Data, ObjectExtensions.ConvertTo(property.PropertyType, TempValue));
        col.SetValue(Data, ObjectExtensions.ConvertTo(col.DataType, TempValue) ?? default!);
        return Task.CompletedTask;
    }
}
