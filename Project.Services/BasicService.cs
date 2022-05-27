using Project.AppCore.Repositories;
using Project.AppCore.Services;
using Project.Common.Attributes;
using Project.Models;
using Project.Models.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Project.Services
{
    [IgnoreAutoInject]
    public class BasicService<T> : IBasicService<T>
    {
        protected readonly IRepository repository;

        public BasicService(IRepository repository)
        {
            this.repository = repository;
        }
        public virtual async Task<IQueryResult<bool>> AddItem(T item)
        {
            var flag = await repository.Table<T>().InsertAsync(item);
            return QueryResult.Return<bool>(flag != null);
        }

        public virtual async Task<IQueryResult<bool>> DeleteItem(Expression<Func<T, bool>> whereLambda)
        {
            var flag = await repository.Table<T>().DeleteAsync(whereLambda);
            return QueryResult.Return<bool>(flag > 0);
        }

        public virtual async Task<IQueryCollectionResult<T>> GetListAsync(GenericRequest<T> req)
        {
            var total = await repository.Table<T>().GetCountAsync(req.Expression);
            var list = await repository.Table<T>().GetListAsync(req.Expression, req.PageIndex, req.PageSize);
            return QueryResult.Success().CollectionResult(list, total);
        }

        public virtual async Task<IQueryResult<bool>> UpdateItem(T item, Expression<Func<T,bool>> primaryKey)
        {
            var flag = await repository.Table<T>().UpdateAsync(item, primaryKey);
            return QueryResult.Return<bool>(flag > 0);
        }

        public virtual async Task<IQueryResult<bool>> UpdateItem(Expression<Func<object>> Expression, Expression<Func<T, bool>> primaryKey)
        {
            var flag = await repository.Table<T>().UpdateAsync(Expression, primaryKey);
            return QueryResult.Return<bool>(flag > 0);
        }
    }
}
