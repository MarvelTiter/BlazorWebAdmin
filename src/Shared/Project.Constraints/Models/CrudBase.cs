using Project.Constraints.Models.Request;
using Project.Constraints.Services;

namespace Project.Constraints.Models;
public class CrudBase<T>(IExpressionContext context) : ICrud<T>
    where T : class, new()
{
    public virtual async Task<QueryCollectionResult<T>> QueryListAsync(GenericRequest<T>? request = null)
    {
        if (request is null)
        {
            var list = await context.Select<T>().ToListAsync();
            return list.CollectionResult();
        }
        else
        {
            var list = await context.Select<T>()
                .Where(request.Expression())
                .Count(out var total)
                .Paging(request.PageIndex, request.PageSize)
                .ToListAsync();
            return list.CollectionResult(total);
        }
    }

    public virtual async Task<QueryResult> InsertAsync(T entity)
    {
        var r = await context.Insert(entity).ExecuteAsync();
        return r > 0;
    }

    public virtual async Task<QueryResult> UpdateAsync(T entity)
    {
        var r = await context.Update(entity).ExecuteAsync();
        return r > 0;
    }

    public virtual async Task<QueryResult> DeleteAsync(T entity)
    {
        var r = await context.Delete(entity).ExecuteAsync();
        return r > 0;
    }

}
