using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Text;

namespace BlazorTemplate.ClientCore.Lockup;

public class EnumLookupProvider : ILookupProvider
{
    
    private global::System.Collections.Frozen.FrozenDictionary<string, LookupEntry> items;

    public EnumLookupProvider()
    {
        var dic = new global::System.Collections.Generic.Dictionary<string, LookupEntry>();
        items = global::System.Collections.Frozen.FrozenDictionary.ToFrozenDictionary(dic);
    }

    public string TypeName => "";

    public string GetDisplayString(string key)
    {
        if (items.TryGetValue(key, out var v) == true)
        {
            return v.DisplayName;
        }
        return key;
    }

    public global::System.Collections.Generic.ICollection<global::BlazorTemplate.ClientCore.Lockup.LookupEntry> GetEntries()
    {
        return items.Values;
    }

    public IEnumerable<LookupEntry> LoadData()
    {
        throw new NotImplementedException();
    }
}
