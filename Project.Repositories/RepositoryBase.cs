using DExpSql;
using MDbContext;
using Project.Common.Attributes;
using Project.IRepositories;
using System.Data;
using System.Linq.Expressions;

namespace Project.Repositories
{
    [IgnoreAutoInject]
    public class RepositoryBase<T> : LightDb, IRepositoryBase<T>
    {
        public virtual Task<int> DeleteAsync(Expression<Func<T, bool>> whereExpression)
        {
            Db.DbSet.Delete<T>()
                .Where(whereExpression);
            return Db.ExecuteAsync();
        }

        public virtual Task<IEnumerable<T>> GetListAsync(Expression<Func<T, bool>> whereExpression, int from = 0, int to = 0)
        {
            if (from > 0 || to > 0)
            {
                Db.DbSet.Select<T>().Where(whereExpression).Paging(from, to);
            }
            else
            {
                Db.DbSet.Select<T>().Where(whereExpression);
            }
            return Db.QueryAsync<T>();
        }

        public virtual Task<T> GetSingleAsync(Expression<Func<T, bool>> whereExpression)
        {
            Db.DbSet.Select<T>().Where(whereExpression);
            return Db.SingleAsync<T>();
        }

        public virtual async Task<T> InsertAsync(T item)
        {
            Db.DbSet.Insert(item);
            var flag = await Db.ExecuteAsync();
            return flag > 0 ? item : default;
        }

        public virtual Task<int> UpdateAsync(T item, Expression<Func<T, bool>> whereExpression)
        {
            Db.DbSet.Update(item).Where(whereExpression);
            return Db.ExecuteAsync();
        }

        public virtual Task<int> UpdateAsync(Expression<Func<object>> updateExpression, Expression<Func<T, bool>> whereExpression)
        {
            Db.DbSet.Update(updateExpression).Where(whereExpression);
            return Db.ExecuteAsync();
        }

        public virtual ExpressionSqlCore<T> Select()
        {
            return Db.DbSet.Select<T>();
        }

        public virtual ExpressionSqlCore<T> Count()
        {
            return Db.DbSet.Count<T>();
        }

        public virtual Task<IEnumerable<M>> Query<M>()
        {
            return Db.QueryAsync<M>();
        }
        public Task<IEnumerable<M>> Query<M>(string sql, object param)
        {
            return Db.QueryAsync<M>(sql, param);
        }
        public virtual Task<DataTable> Table()
        {
            return Db.QueryDataTableAsync();
        }

        public Task<DataTable> Table(string sql, object param)
        {
            return Db.QueryDataTableAsync(sql, param);
        }
        public virtual Task<M> Single<M>()
        {
            return Db.SingleAsync<M>();
        }
        public Task<M> Single<M>(string sql, object param)
        {
            return Db.SingleAsync<M>(sql, param);
        }
        public Task<M> Request<M>(Func<DbContext, Task<M>> func)
        {
            return func.Invoke(Db);
        }
    }
}
