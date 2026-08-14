using System.Collections.Frozen;

namespace BlazorTemplate.ClientCore.Lockup;

public abstract class BaseLookupProvider : ILookupProvider
{
    protected BaseLookupProvider()
    {
        EntryCache = LoadData().ToFrozenDictionary(l => l.Code);
        if (RefreshInterval > TimeSpan.Zero)
        {
            // TODO: 启用刷新
            
        }
    }
    protected FrozenDictionary<string, LookupEntry> EntryCache { get; set; }
    protected virtual TimeSpan RefreshInterval => TimeSpan.Zero;
    public abstract string TypeName { get; }
    public virtual string GetDisplayString(string key)
    {
        if (EntryCache.TryGetValue(key, out var v) == true)
        {
            return v.DisplayName;
        }
        return key;
    }
    public virtual ICollection<LookupEntry> GetEntries() => EntryCache.Values;
    public abstract IEnumerable<LookupEntry> LoadData();
}
