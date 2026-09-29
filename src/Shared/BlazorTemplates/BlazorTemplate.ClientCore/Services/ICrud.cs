namespace BlazorTemplate.ClientCore.Services;

public interface ICrud<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors| DynamicallyAccessedMemberTypes.PublicProperties)]T>
{
    Task<QueryCollectionResult<T>> QueryListAsync(GenericRequest<T>? request = null);
    Task<QueryResult> InsertAsync(T entity);
    Task<QueryResult> UpdateAsync(T entity);
    Task<QueryResult> DeleteAsync(T entity);
}
