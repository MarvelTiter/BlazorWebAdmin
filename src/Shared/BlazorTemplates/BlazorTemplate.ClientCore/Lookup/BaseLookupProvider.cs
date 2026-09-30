using System.Collections.Frozen;

namespace BlazorTemplate.ClientCore.Lookup;

/// <summary>
/// 一个分组及其条目。provider 需要同时提供多种数据（如多个字典类型）时用它作为返回单元。
/// </summary>
public sealed class LookupCategory(string category, IEnumerable<LookupEntry> entries) : List<LookupEntry>(entries)
{
    /// <summary>分组名，对应 <c>ILookupService</c> 的 <c>category</c> 参数。</summary>
    public string Category { get; } = category;
}

/// <summary>
///     自定义数据源基类。
///     <para>
///         派生类只需实现 <see cref="LoadDataAsync" /> 一个成员；缓存、并发串行化、新鲜度判断、
///         后台刷新循环都由基类负责。同步读接口只读缓存，绝不触发加载。
///     </para>
///     <para>
///         只有一组数据时不要构造 <see cref="LookupCategory" />，用 <see cref="Single" /> 包一层即可。
///     </para>
/// </summary>
public abstract class BaseLookupProvider : ILookupProvider, IAsyncDisposable
{
    private const string DEFAULT_CATEGORY = "default_category";

    private static readonly FrozenDictionary<string, FrozenDictionary<string, LookupEntry>> empty =
        new Dictionary<string, FrozenDictionary<string, LookupEntry>>().ToFrozenDictionary();

    private readonly SemaphoreSlim gate = new(1, 1);
    private readonly CancellationTokenSource loadCts = new();
    private readonly Task loadTask;

    private volatile FrozenDictionary<string, FrozenDictionary<string, LookupEntry>> cache = empty;
    private DateTimeOffset loadedAt = DateTimeOffset.MinValue;

    protected BaseLookupProvider()
    {
        loadTask = Task.Factory.StartNew(async () =>
        {
            while (!loadCts.IsCancellationRequested)
            {
                await LoadAsync(loadCts.Token).ConfigureAwait(false);
                if (RefreshInterval <= TimeSpan.Zero)
                {
                    return;
                }
                await Task.Delay(RefreshInterval, loadCts.Token).ConfigureAwait(false);
            }
        }, TaskCreationOptions.LongRunning);
    }

    public abstract string TypeName { get; }

    /// <summary>刷新周期；<c>&lt;= TimeSpan.Zero</c> 表示只在启动时加载一次。</summary>
    protected virtual TimeSpan RefreshInterval => TimeSpan.Zero;

    /// <summary>缓存是否仍在新鲜期内（已加载且未超过 <see cref="RefreshInterval" />）。</summary>
    public bool IsFresh => loadedAt != DateTimeOffset.MinValue
        && (RefreshInterval <= TimeSpan.Zero || DateTimeOffset.UtcNow - loadedAt < RefreshInterval);

    public virtual string GetDisplayString(string key)
    {
        if (cache.TryGetValue(DEFAULT_CATEGORY, out var v) == true && v.TryGetValue(key, out var e) == true)
        {
            return e.DisplayName;
        }
        return key;
    }

    public virtual string GetDisplayString(string category, string key)
    {
        if (cache.TryGetValue(category, out var v) == true && v.TryGetValue(key, out var e) == true)
        {
            return e.DisplayName;
        }
        return key;
    }

    /// <summary>所有分组的条目并集；单组 provider 即该组的全部条目。</summary>
    public virtual ICollection<LookupEntry> GetEntries() => [.. cache.Values.SelectMany(c => c.Values)];

    public virtual ICollection<LookupEntry> GetEntries(string category) => cache.GetValueOrDefault(category)?.Values ?? [];

    /// <summary>仍在新鲜期内则短路，否则加载。</summary>
    private async ValueTask LoadAsync(CancellationToken cancellationToken = default)
    {
        if (IsFresh)
        {
            return;
        }
        await ReloadAsync(cancellationToken).ConfigureAwait(false);
    }

    /// <summary>强制重新加载；并发调用会被串行化，不会重复取数。</summary>
    private async ValueTask ReloadAsync(CancellationToken cancellationToken = default)
    {
        await gate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            var data = await LoadDataAsync(cancellationToken).ConfigureAwait(false);
            cache = data.GroupBy(c => c.Category)
                .ToFrozenDictionary(g => g.Key, g => g.SelectMany(c => c).ToFrozenDictionary(e => e.Code));
            loadedAt = DateTimeOffset.UtcNow;
        }
        finally
        {
            gate.Release();
        }
    }

    /// <summary>
    /// 从数据源异步取全量数据，按分组返回。
    /// </summary>
    protected abstract ValueTask<IList<LookupCategory>> LoadDataAsync(CancellationToken cancellationToken);

    protected static IList<LookupCategory> Single(IEnumerable<LookupEntry> datas) => [new LookupCategory(DEFAULT_CATEGORY, datas)];

    public async ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);
        await loadCts.CancelAsync();
        try
        {
            await loadTask;
        }
        finally
        {
            loadTask.Dispose();
            loadCts.Dispose();
        }
    }
}
