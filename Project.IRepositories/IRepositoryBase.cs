using System.Linq.Expressions;

namespace Project.IRepositories
{
    public interface IRepositoryBase<T>
    {
        //T Insert(T item);
        //bool Update(T item);
        //bool Delete(T item);
        //T GetSingle(Q req);
        //IEnumerable<T> GetList(Q req);
        Task<T> InsertAsync(T item);
        Task<int> UpdateAsync(T item, Expression<Func<T, bool>> whereExpression);
        Task<int> UpdateAsync(Expression<Func<object>> updateExpression, Expression<Func<T, bool>> whereExpression);
        Task<int> DeleteAsync(Expression<Func<T, bool>> whereExpression);
        Task<T> GetSingleAsync(Expression<Func<T, bool>> whereExpression);
        Task<IEnumerable<T>> GetListAsync(Expression<Func<T, bool>> whereExpression);
    }
}
