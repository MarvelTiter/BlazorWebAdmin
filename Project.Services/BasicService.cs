using MDbContext.ExpressionSql;
using MDbContext.Repository;
using Project.AppCore.Services;
using Project.Common.Attributes;
using Project.Models;
using Project.Models.Request;
using System.Linq.Expressions;

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
            return (flag != null).Result();
        }

        public virtual async Task<IQueryResult<bool>> DeleteItem(Expression<Func<T, bool>> whereLambda)
        {
            var flag = await context.Repository<T>().DeleteAsync(whereLambda);
            return (flag > 0).Result();
        }

        public virtual async Task<IQueryCollectionResult<T>> GetListAsync(GenericRequest<T> req)
        {
            var list = await context.Repository<T>().GetListAsync(req.Expression, out var total, req.PageIndex, req.PageSize);
            return list.CollectionResult((int)total);
        }

        public async Task<IQueryResult<T>> GetSingleAsync(Expression<Func<T, bool>> whereExp)
        {
            var result = await context.Repository<T>().GetSingleAsync(whereExp);
            return result?.Result();
        }

        public virtual async Task<IQueryResult<bool>> UpdateItem(T item, Expression<Func<T, bool>> primaryKey)
        {
            var flag = await context.Repository<T>().UpdateAsync(item, primaryKey);
            return (flag > 0).Result();
        }

        public virtual async Task<IQueryResult<bool>> UpdateItem(Expression<Func<object>> exp, Expression<Func<T, bool>> primaryKey)
        {
            var flag = await context.Repository<T>().UpdateAsync(exp, primaryKey);
            return (flag > 0).Result();
        }
    }
}
