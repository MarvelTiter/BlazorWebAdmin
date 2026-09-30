using Microsoft.Extensions.DependencyInjection;
using System.Collections.Concurrent;
using System.Collections.Frozen;

namespace BlazorTemplate.ClientCore.Lookup;

public class LookupService : ILookupService
{
    private static readonly FrozenDictionary<string, ILookupProvider> emptyLookups =
        new Dictionary<string, ILookupProvider>().ToFrozenDictionary();

    /// <summary>
    ///     枚举 provider 的静态注册表：由生成代码的模块初始化器经 <see cref="AddEnumProvider" /> 写入。
    /// </summary>
    private static readonly ConcurrentDictionary<string, ILookupProvider> staticProviders = [];

    private FrozenDictionary<string, ILookupProvider> lookups = emptyLookups;
    private readonly IServiceProvider serviceProvider;
    private readonly ILogger<LookupService>? logger;

    public LookupService(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
        logger = serviceProvider.GetService<ILogger<LookupService>>();
        Build();
    }

    /// <summary>
    /// 登记 provider：静态枚举 provider + 当前作用域内的 DI provider。
    /// </summary>
    public void Build()
    {
        var map = new Dictionary<string, ILookupProvider>(staticProviders);
        foreach (var item in serviceProvider.GetServices<ILookupProvider>())
        {
            map[item.TypeName] = item;
        }
        lookups = map.ToFrozenDictionary();
    }

    public static void AddEnumProvider(ILookupProvider provider)
    {
        staticProviders.AddOrUpdate(provider.TypeName, provider, (_, _) => provider);
    }

    public string GetDisplayString(string type, string? key)
    {
        if (key is not { })
        {
            return string.Empty;
        }
        if (lookups.TryGetValue(type, out var l) == true)
        {
            return l.GetDisplayString(key);
        }
        return key;
    }

    public string GetDisplayString(string type, string category, string? key)
    {
        if (key is not { })
        {
            return string.Empty;
        }
        if (lookups.TryGetValue(type, out var l) == true)
        {
            return l.GetDisplayString(category, key);
        }
        return key;
    }

    public ICollection<LookupEntry> GetEntries(string type)
    {
        if (lookups.TryGetValue(type, out var l) == true)
        {
            return l.GetEntries();
        }
        return [];
    }
    public ICollection<LookupEntry> GetEntries(string type, string category)
    {
        if (lookups.TryGetValue(type, out var l) == true)
        {
            return l.GetEntries(category);
        }
        return [];
    }
}
