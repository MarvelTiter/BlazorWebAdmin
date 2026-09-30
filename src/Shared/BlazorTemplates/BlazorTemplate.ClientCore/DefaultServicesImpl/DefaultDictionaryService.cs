using BlazorTemplate.ClientCore.Models.Dictionary;

namespace BlazorTemplate.ClientCore.DefaultServicesImpl;

/// <summary>
/// 数据字典默认实现（单表自关联，无限级树）。
/// </summary>
public class DefaultDictionaryService<
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicProperties)] TDictItem>
    : IDictionaryService<TDictItem>
    where TDictItem : class, IDictItem, new()
{
    protected readonly IExpressionContext context;

    public DefaultDictionaryService(IExpressionContext context)
    {
        this.context = context;
    }

    public async Task<QueryCollectionResult<TDictItem>> GetDictTypesAsync()
    {
        var list = await context.Select<TDictItem>()
            .Where(t => t.ParentId == null || t.ParentId == "ROOT")
            .ToListAsync();
        return list.CollectionResult();
    }

    public async Task<QueryCollectionResult<TDictItem>> GetAllItemAsync()
    {
        var list = await context.Select<TDictItem>().Where(t => t.ParentId != null && t.ParentId != "ROOT").ToListAsync();
        return list.CollectionResult();
    }

    public async Task<QueryCollectionResult<TDictItem>> GetDictItemListAsync(DictRequestContext<TDictItem> query)
    {
        var req = query.Request;
        var parent = query.Parent;
        var list = await context.Select<TDictItem>()
            .Where(req.Expression())
            .WhereIf(parent is not null, t => t.ParentId == parent!.ItemId)
            .Count(out var total)
            .Paging(req.PageIndex, req.PageSize)
            .ToListAsync();
        return list.CollectionResult((int)total);
    }

    public async Task<QueryResult> InsertDictItemAsync(TDictItem item)
    {
        var i = await context.Insert(item).OrUpdate().ExecuteAsync();
        return i > 0;
    }

    public async Task<QueryResult> UpdateDictItemAsync(TDictItem item)
    {
        var i = await context.Update(item).Where(t => t.ItemId == item.ItemId).ExecuteAsync();
        return i > 0;
    }

    public async Task<QueryResult> DeleteDictItemAsync(TDictItem item)
    {
        using var scoped = context.CreateMainDbScoped();
        try
        {
            await scoped.BeginTransactionAsync();
            // 删除该节点及其所有子孙节点
            await DeleteTreeAsync(scoped, item.ItemId!);
            await scoped.CommitTransactionAsync();
            return QueryResult.Success();
        }
        catch (Exception)
        {
            await scoped.RollbackTransactionAsync();
            throw;
        }
    }

    private async Task DeleteTreeAsync(ISingleScopedExpressionContext scoped, string itemId)
    {
        // 先找子节点
        var children = await scoped.Select<TDictItem>()
            .Where(i => i.ParentId == itemId)
            .ToListAsync();
        foreach (var child in children)
        {
            await DeleteTreeAsync(scoped, child.ItemId!);
        }
        await scoped.Delete<TDictItem>().Where(i => i.ItemId == itemId).ExecuteAsync();
    }
}
