namespace BlazorTemplate.ClientCore.Models;

public class CrudBase<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicProperties)] T>(IExpressionContext context) : ICrud<T>
    where T : class, new()
{
    protected readonly IExpressionContext context = context;

    protected virtual ITransientContext DefaultContext => context.SwitchDatabase("MainDb");
    
    public virtual async Task<QueryCollectionResult<T>> QueryListAsync(GenericRequest<T>? request = null)
    {
        if (request is null)
        {
            var list = await DefaultContext.Select<T>().ToListAsync();
            return list.CollectionResult();
        }
        else
        {
            var list = await DefaultContext.Select<T>()
                .Where(request.Expression())
                .Count(out var total)
                .Paging(request.PageIndex, request.PageSize)
                .ToListAsync();
            return list.CollectionResult(total);
        }
    }

    public virtual async Task<QueryResult> InsertAsync(T entity)
    {
        var r = await DefaultContext.Insert(entity).ExecuteAsync();
        return r > 0;
    }

    public virtual async Task<QueryResult> UpdateAsync(T entity)
    {
        var r = await DefaultContext.Update(entity).ExecuteAsync();
        return r > 0;
    }

    public virtual async Task<QueryResult> DeleteAsync(T entity)
    {
        var r = await DefaultContext.Delete(entity).ExecuteAsync();
        return r > 0;
    }
}