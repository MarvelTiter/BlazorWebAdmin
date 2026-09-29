using System.Collections.Frozen;

namespace BlazorTemplate.ClientCore.Lookup;

/// <summary>
///     把枚举元数据适配成 <see cref="ILookupProvider" />，是<strong>唯一</strong>的枚举 provider 实现。
///     <para>
///         取代原先由生成器为每个枚举产出的 <c>{Name}LookupProvider</c> 类：provider 的职责只是
///         "把枚举元数据翻译成 <see cref="LookupEntry" />"，与具体枚举无关，因此不需要为每个枚举各生成一份。
///         生成代码只负责建元数据，然后写出 <c>new EnumLookupProvider&lt;全局枚举名&gt;(meta)</c> 即可。
///     </para>
///     <para>
///         行为与原生成代码逐条对齐：
///         <list type="bullet">
///             <item>
///                 <see cref="TypeName" /> 取枚举<strong>简单名</strong>——消费方拿 <c>ColumnInfo.LookupType</c>
///                 （= <c>enumType.Name</c>）查 <c>LookupService</c>，两边必须一致；
///             </item>
///             <item>
///                 只收录带 <c>[Display]</c> 的成员（<see cref="EnumField.Label" /> 非空），
///                 <c>Code</c> = 成员名、<c>DisplayName</c> = 标签；
///             </item>
///             <item>
///                 一个带 <c>[Display]</c> 的成员都没有时生成代码不会注册它，等价于原来"不生成 provider"；
///             </item>
///             <item>
///                 <see cref="LoadData" /> 维持原样抛 <see cref="NotImplementedException" />——数据在构造时
///                 就已经从元数据构建完成，没有延迟加载需求。
///             </item>
///         </list>
///     </para>
///     <para>
///         <typeparamref name="TEnum" /> 在这里是<strong>封闭</strong>的（由生成代码写出具体枚举名），
///         因此 <c>typeof(TEnum).Name</c> 与泛型实例化都是编译期可定的，不会引入 <c>MakeGenericType</c>。
///     </para>
/// </summary>
/// <typeparam name="TEnum">被适配的枚举类型；同时决定 <see cref="TypeName" />。</typeparam>
public sealed class EnumLookupProvider<TEnum> : ILookupProvider
    where TEnum : struct, Enum
{
    private readonly FrozenDictionary<string, LookupEntry> items;

    /// <summary>从枚举元数据构建下拉项。只会收录带 <c>[Display]</c> 的成员。</summary>
    public EnumLookupProvider(IEnumMeta meta)
    {
        ArgumentNullException.ThrowIfNull(meta);

        var dic = new Dictionary<string, LookupEntry>();
        foreach (var field in meta.Fields)
        {
            if (field.Label is not null)
            {
                dic[field.Name] = new LookupEntry(field.Name, field.Label);
            }
        }

        items = dic.ToFrozenDictionary();
    }

    /// <inheritdoc />
    public string TypeName => typeof(TEnum).Name;

    /// <inheritdoc />
    public string GetDisplayString(string key)
    {
        if (items.TryGetValue(key, out var v) == true)
        {
            return v.DisplayName;
        }
        return key;
    }

    /// <inheritdoc />
    public ICollection<LookupEntry> GetEntries() => items.Values;

    /// <inheritdoc />
    public IEnumerable<LookupEntry> LoadData()
    {
        throw new NotImplementedException();
    }
}
