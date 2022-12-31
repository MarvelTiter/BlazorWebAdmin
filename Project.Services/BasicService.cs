using LogAopCodeGenerator;
using MDbContext.ExpressionSql;
using MDbContext.Repository;
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
        protected readonly IExpressionContext context;

        public BasicService(IExpressionContext context)
        {
            this.context = context;
        }
        public virtual async Task<IQueryResult<bool>> AddItem(T item)
        {
            var flag = await context.Repository<T>().InsertAsync(item);
            return QueryResult.Return<bool>(flag != null);
        }

        public virtual async Task<IQueryResult<bool>> DeleteItem(Expression<Func<T, bool>> whereLambda)
        {
            var flag = await context.Repository<T>().DeleteAsync(whereLambda);
            return QueryResult.Return<bool>(flag > 0);
        }

        public virtual async Task<IQueryCollectionResult<T>> GetListAsync(GenericRequest<T> req)
        {
            var list = await context.Repository<T>().GetListAsync(req.Expression, out var total, req.PageIndex, req.PageSize);
            return QueryResult.Success().CollectionResult(list, (int)total);
        }

        public virtual async Task<IQueryResult<bool>> UpdateItem(T item, Expression<Func<T, bool>> primaryKey)
        {
            var flag = await context.Repository<T>().UpdateAsync(item, primaryKey);
            return QueryResult.Return<bool>(flag > 0);
        }

        public virtual async Task<IQueryResult<bool>> UpdateItem(Expression<Func<object>> exp, Expression<Func<T, bool>> primaryKey)
        {
            var flag = await context.Repository<T>().UpdateAsync(exp, primaryKey);
            return QueryResult.Return<bool>(flag > 0);
        }
    }
}
