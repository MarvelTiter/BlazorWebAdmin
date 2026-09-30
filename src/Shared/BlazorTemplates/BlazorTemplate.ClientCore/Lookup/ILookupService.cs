namespace BlazorTemplate.ClientCore.Lookup;

public readonly record struct LookupEntry(string Code, string DisplayName);

public interface ILookupService
{
    string GetDisplayString(string type, string? key);
    string GetDisplayString(string type, string category, string? key);
    ICollection<LookupEntry> GetEntries(string type);
    ICollection<LookupEntry> GetEntries(string type, string category);
}

/// <summary>
///     单个查找数据源。
///     <para>
///         category 是<strong>可选能力</strong>：一个 provider 可以只提供一组数据（此时实现前三个成员即可），
///         也可以像数据字典那样用 category 区分多组数据（此时再覆写后两个成员）。
///     </para>
/// </summary>
public interface ILookupProvider
{
    /// <summary>注册键，消费方拿 <c>ColumnInfo.LookupType</c> 与它匹配。</summary>
    string TypeName { get; }

    /// <summary>同步取显示名：只读缓存，不触发加载。</summary>
    string GetDisplayString(string key);

    /// <summary>同步取条目：只读缓存，不触发加载。</summary>
    ICollection<LookupEntry> GetEntries();

    /// <summary>
    ///     按分组取显示名。不提供多组数据的 provider 不必实现——默认忽略 <paramref name="category" />。
    /// </summary>
    string GetDisplayString(string category, string key) => GetDisplayString(key);

    /// <summary>
    ///     按分组取条目。不提供多组数据的 provider 不必实现——默认忽略 <paramref name="category" />。
    /// </summary>
    ICollection<LookupEntry> GetEntries(string category) => GetEntries();
}
