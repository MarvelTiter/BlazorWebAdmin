using BlazorTemplate.Constraints.Models.Request;

namespace BlazorTemplate.Constraints.Services;

public interface ICrud<T>
{
    Task<QueryCollectionResult<T>> QueryListAsync(GenericRequest<T>? request = null);
    Task<QueryResult> InsertAsync(T entity);
    Task<QueryResult> UpdateAsync(T entity);
    Task<QueryResult> DeleteAsync(T entity);
}
