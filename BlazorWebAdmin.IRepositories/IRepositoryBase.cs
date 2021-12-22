namespace BlazorWebAdmin.IRepositories
{
    public interface IRepositoryBase<T, Q>
    {
        //T Insert(T item);
        //bool Update(T item);
        //bool Delete(T item);
        //T GetSingle(Q req);
        //IEnumerable<T> GetList(Q req);
        Task<T> InsertAsync(T item);
        Task<bool> UpdateAsync(T item);
        Task<bool> DeleteAsync(T item);
        Task<T> GetSingleAsync(Q req);
        Task<IEnumerable<T>> GetListAsync(Q req);
    }
}
