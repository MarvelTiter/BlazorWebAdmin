using Project.Constraints.Models.Request;

namespace Project.Constraints.Services;

public interface ICrud<T>
{
    Task<QueryCollectionResult<T>> QueryListAsync(GenericRequest<T>? request = null);
    Task<QueryResult> InsertAsync(T entity);
    Task<QueryResult> UpdateAsync(T entity);
    Task<QueryResult> DeleteAsync(T entity);
}
