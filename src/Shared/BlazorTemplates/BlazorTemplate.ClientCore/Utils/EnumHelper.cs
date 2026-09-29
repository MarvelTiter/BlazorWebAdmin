using BlazorTemplate.ClientCore.Lookup;

namespace BlazorTemplate.ClientCore.Utils;

/// <summary>
///     枚举的强类型静态访问入口（取值集合、显示名、成员名字典）。
///     <para>
///         实现已<strong>完全去反射</strong>：编译期由 <c>EnumDisplayProviderGenerator</c> 为枚举生成
///         <see cref="IEnumMeta" />，经模块初始化器注册到 <see cref="EnumMetaRegistry" />；
///         这里只做一次按 <see cref="Type" /> 的字典查询与逐项拆箱。
///     </para>
///     <para>
///         原实现的两处反射在 AOT / 裁剪下均报错，现已消除：
///         <c>Enum.GetValues(Type)</c>（IL3050）、
///         <c>GetField(...).GetCustomAttribute&lt;DisplayAttribute&gt;()</c>（IL2075）。
///     </para>
///     <para>
///         覆盖范围：生成器只扫描<strong>当前编译单元</strong>的枚举声明。引用程序集里定义的枚举
///         默认没有元数据，需要在其消费方调用一次 <c>LookupServiceBuilder.AddEnumLookupProvider&lt;TEnum&gt;()</c>
///         作为编译期登记标记（该方法本身无运行时行为，是纯标记）。
///         未被覆盖时 <see cref="GetValues" /> 返回空集合、<see cref="GetDisplayName" /> 返回
///         <see langword="null" />、<see cref="ParseDictionary" /> 返回空字典。
///     </para>
/// </summary>
/// <typeparam name="TEnum">
///     枚举类型，<strong>刻意不加 <c>where TEnum : struct, Enum</c> 约束</strong>：
///     <c>FluentEnumSelect&lt;TEnum&gt;</c> 以 <c>TEnum?</c> 承载绑定值，因此 <c>LayoutMode?</c>
///     这类可空枚举类型实参必须合法。
/// </typeparam>
public static class EnumHelper<TEnum>
{
    private static readonly Type enumType = Nullable.GetUnderlyingType(typeof(TEnum)) ?? typeof(TEnum);
    private static readonly IEnumMeta? meta = EnumMetaRegistry.TryGet(enumType, out var m) ? m : null;
    private static readonly TEnum[] values = ToTypedValues(meta);

    /// <summary>
    ///     把元数据里的成员值逐项转成 <typeparamref name="TEnum" />。
    ///     <para>
    ///         <c>(TEnum)item</c> 是<strong>拆箱</strong>，不是泛型实例化：<typeparamref name="TEnum" /> 为
    ///         <c>LayoutMode?</c> 时也能正确拆箱（CLR 支持从装箱的 <c>T</c> 拆到 <c>T?</c>），
    ///         所以这里不需要任何 <c>MakeGenericMethod</c>。也因此 <see cref="IEnumMeta" /> 可以保持类型擦除。
    ///     </para>
    /// </summary>
    private static TEnum[] ToTypedValues(IEnumMeta? meta)
    {
        if (meta is null)
        {
            return [];
        }

        var fields = meta.Fields;
        var result = new TEnum[fields.Count];
        for (var i = 0; i < fields.Count; i++)
        {
            result[i] = (TEnum)fields[i].Value;
        }

        return result;
    }

    /// <summary>枚举的全部成员，按底层值升序（与 <c>Enum.GetValues</c> 一致）。</summary>
    public static IEnumerable<TEnum> GetValues() => values;

    /// <summary>取成员显示名；未声明 <c>[Display]</c> 或值未定义时返回 <see langword="null" />。</summary>
    public static string? GetDisplayName(TEnum value)
    {
        if (meta is null || value is null)
        {
            return null;
        }

        return meta.GetDisplayName(value);
    }

    /// <summary>
    ///     成员名 → 枚举值。沿用原 <c>Enum.TryParse(Type, string, out object)</c> 语义：
    ///     成员名、数字串、<c>Flags</c> 的逗号串都支持（该方法实测未标 <c>[RequiresDynamicCode]</c>，AOT 安全）。
    /// </summary>
    public static TEnum? Parse(string value)
    {
        if (Enum.TryParse(enumType, value, out var enumValue))
        {
            return (TEnum)enumValue;
        }
        return default;
    }

    /// <summary>成员名 → 显示名，覆盖全部成员；没有 <c>[Display]</c> 的成员取成员名。</summary>
    public static Dictionary<string, string> ParseDictionary()
        => meta is null ? [] : new Dictionary<string, string>(meta.DisplayNames);
}
