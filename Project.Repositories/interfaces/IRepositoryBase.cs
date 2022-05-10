using System.Data;
using System.Linq.Expressions;
using DExpSql;
using Project.Common.Attributes;

namespace Project.Repositories.interfaces
{
    [IgnoreAutoInject]
    public interface IRepositoryBase<T>
    {
        //T Insert(T item);
        //bool Update(T item);
        //bool Delete(T item);
        //T GetSingle(Q req);
        //IEnumerable<T> GetList(Q req);
        Task<T> InsertAsync(T item);
        Task<int> UpdateAsync(T item, Expression<Func<T, bool>>? whereExpression);
        Task<int> UpdateAsync(Expression<Func<object>> updateExpression, Expression<Func<T, bool>>? whereExpression);
        Task<int> DeleteAsync(Expression<Func<T, bool>>? whereExpression);
        Task<T> GetSingleAsync(Expression<Func<T, bool>>? whereExpression);
        Task<IEnumerable<T>> GetListAsync(Expression<Func<T, bool>>? whereExpression, int index = 0, int size = 0);
        Task<int> GetCountAsync(Expression<Func<T, bool>>? whereExpression);
        Task<M> Request<M>(Func<MDbContext.DbContext, Task<M>> func);
        ExpressionSqlCore<T> Select();
        ExpressionSqlCore<T> Count();
        Task<IEnumerable<M>> Query<M>();
        Task<IEnumerable<M>> Query<M>(string sql, object param);
        Task<M> Single<M>();
        Task<M> Single<M>(string sql, object param);
        Task<DataTable> Table();
        Task<DataTable> Table(string sql, object param);
    }
}
