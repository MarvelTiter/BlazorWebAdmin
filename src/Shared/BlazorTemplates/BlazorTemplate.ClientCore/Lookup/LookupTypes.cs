namespace BlazorTemplate.ClientCore.Lookup;

/// <summary>
///     内置的 lookup 类型名，对应 <c>ILookupService</c> 的 <c>type</c> 参数。
/// </summary>
public static class LookupTypes
{
    /// <summary>
    ///     数据字典。具体字典类型由 <c>category</c> 参数区分，所以整张字典表只需要一个 provider。
    /// </summary>
    public const string Dictionary = "DictionaryLookup";
}
