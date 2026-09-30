using System.Collections.Frozen;

namespace BlazorTemplate.ClientCore.Lookup;

/// <summary>
/// 把枚举元数据适配成 <see cref="ILookupProvider" />
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
}
