namespace BlazorWebAdmin.IRepositories
{
    public interface IRepositoryBase<T, Q>
    {
        T Insert(T item);
        bool Update(T item);
        bool Delete(T item);
        T GetSingle(Q req);
        IEnumerable<T> GetList(Q req);
    }
}
