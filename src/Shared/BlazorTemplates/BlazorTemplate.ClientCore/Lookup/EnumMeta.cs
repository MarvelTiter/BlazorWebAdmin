using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

namespace BlazorTemplate.ClientCore.Lookup;

/// <summary>
///     枚举的一个成员，是编译器可静态枚举的最小单位。
/// </summary>
/// <param name="Value">成员值（装箱）。</param>
/// <param name="Name">成员名，如 <c>DayStart</c>。</param>
/// <param name="Label">
///     <c>[Display]</c> 声明的标签；未声明时为 <see langword="null" />。
///     <para>
///         这一层区分必须保留：下拉要"可选项"语义，只列带 <c>[Display]</c> 的成员；
///         而 <c>EnumHelper.ParseDictionary</c> 要"枚举字典"语义，需列出全部成员并把缺标签的回退成成员名。
///         若只存合并后的显示名，两种语义就无法在同一份数据上区分。
///     </para>
/// </param>
public readonly record struct EnumField(object Value, string Name, string? Label);

/// <summary>
///     单个枚举的编译期元数据，是枚举信息的<strong>唯一底层来源</strong>。
///     <para>
///         由 <c>EnumDisplayProviderGenerator</c> 为枚举生成实现，并在模块初始化时注册到
///         <see cref="EnumMetaRegistry" />。上层两类消费方都从它派生，不再各自解析枚举：
///         <list type="bullet">
///             <item>
///                 <c>EnumHelper&lt;TEnum&gt;</c>——"枚举字典"语义（全部成员，缺 <c>[Display]</c> 回退成员名）；
///             </item>
///             <item>
///                 <see cref="EnumLookupProvider{TEnum}" />——"可选项"语义（只含带 <c>[Display]</c> 的成员），
///                 把它翻译成 <see cref="LookupEntry" /> 交给 <c>LookupService</c>。
///             </item>
///         </list>
///     </para>
///     <para>
///         <strong>为什么不声明成 <c>IEnumMeta&lt;TEnum&gt;</c>：</strong>
///         注册表必须按<strong>运行时 <see cref="Type" /></strong> 查表（枚举简单名会跨命名空间重复，不能做键），
///         而 <c>EnumHelper&lt;TEnum&gt;</c> 的 <c>TEnum</c> 是<strong>无约束</strong>的——
///         既可能是具体枚举，也可能是 <c>LayoutMode?</c>（见 <c>FluentEnumSelect&lt;TEnum&gt;</c> 的
///         <c>[Parameter] TEnum? EnumValue</c>）。要从 <see cref="Type" /> 走到 <c>IEnumMeta&lt;TEnum&gt;</c>
///         只能靠 <c>MakeGenericType</c>，那正是本次 AOT 治理要消除的 R2 硬阻塞（IL3050 + IL2060）；
///         若改为给 <c>TEnum</c> 加 <c>where TEnum : struct, Enum</c> 约束，可空枚举类型实参又会全部编译不过。
///         因此这里**刻意保持类型擦除**：泛型只出现在实参可静态写死的地方（见 <see cref="EnumLookupProvider{TEnum}" />），
///         值则以 <see cref="EnumField.Value" /> 装箱承载、由消费侧拆箱。
///     </para>
/// </summary>
public interface IEnumMeta
{
    /// <summary>全部成员，按底层值升序（与 <c>Enum.GetValues</c> 的顺序一致）。</summary>
    IReadOnlyList<EnumField> Fields { get; }

    /// <summary>
    ///     成员名 → 显示名，覆盖<strong>全部</strong>成员；未声明 <c>[Display]</c> 的成员取成员名。
    /// </summary>
    IReadOnlyDictionary<string, string> DisplayNames { get; }

    /// <summary>
    ///     取 <c>[Display]</c> 标签；未声明 <c>[Display]</c>、或值不在枚举定义内时返回
    ///     <see langword="null" />（与原反射实现 <c>GetCustomAttribute&lt;DisplayAttribute&gt;()?.GetName()</c> 一致）。
    /// </summary>
    string? GetDisplayName(object value);
}

/// <summary>
///     枚举元数据的静态注册表，按枚举的 <see cref="Type" /> 为键。
///     <para>
///         用类型而非简单名作键：枚举的简单名可能在不同命名空间重复，而 <c>typeof(TEnum)</c>
///         在 AOT 下是安全常量，不引入任何反射。
///     </para>
/// </summary>
public static class EnumMetaRegistry
{
    private static readonly ConcurrentDictionary<Type, IEnumMeta> metas = new();

    /// <summary>由生成代码的模块初始化器调用；同名重复注册时以最后一次为准。</summary>
    public static void Register(Type enumType, IEnumMeta meta)
    {
        ArgumentNullException.ThrowIfNull(enumType);
        ArgumentNullException.ThrowIfNull(meta);
        metas[enumType] = meta;
    }

    /// <summary>查询枚举的编译期元数据；未被生成器覆盖的枚举返回 <see langword="false" />。</summary>
    public static bool TryGet(Type enumType, [NotNullWhen(true)] out IEnumMeta? meta)
    {
        if (metas.TryGetValue(enumType, out var found))
        {
            meta = found;
            return true;
        }

        meta = null;
        return false;
    }
}
