using Microsoft.Extensions.DependencyInjection;
using System.Collections.Concurrent;
using System.Collections.Frozen;

namespace BlazorTemplate.ClientCore.Lockup;

public class LookupService : ILookupService
{
    private FrozenDictionary<string, ILookupProvider>? lookups;
    private readonly IServiceProvider serviceProvider;
    private static readonly ConcurrentDictionary<string, ILookupProvider> staticProviders = [];

    public LookupService(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
        Build();
    }

    public void Build()
    {
        var providers = serviceProvider.GetServices<ILookupProvider>();
        foreach (var item in providers)
        {
            if (!staticProviders.TryGetValue(item.TypeName, out _))
            {
                item.LoadData();
                staticProviders.TryAdd(item.TypeName, item);
            }
        }
        lookups = staticProviders.ToFrozenDictionary();
    }

    public static void AddEnumProvider(ILookupProvider provider)
    {
        staticProviders.AddOrUpdate(provider.TypeName, provider, (k, e) => provider);
    }

    public string GetDisplayString(string type, string? key)
    {
        if (key is not { })
        {
            return string.Empty;
        }
        if (lookups?.TryGetValue(type, out var l) == true)
        {
            return l.GetDisplayString(key);
        }
        return key;
    }

    public ICollection<LookupEntry> GetEntries(string type)
    {
        if (lookups?.TryGetValue(type, out var l) == true)
        {
            return l.GetEntries();
        }
        return [];
    }

}
