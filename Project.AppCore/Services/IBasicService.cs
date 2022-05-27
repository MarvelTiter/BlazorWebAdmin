using Project.Models;
using Project.Models.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Project.AppCore.Services
{
    public partial interface IBasicService<T>
    {
        Task<IQueryCollectionResult<T>> GetListAsync(GenericRequest<T> req);
        Task<IQueryResult<bool>> AddItem(T item);
        Task<IQueryResult<bool>> UpdateItem(T item, Expression<Func<T, bool>> primaryKey);
        Task<IQueryResult<bool>> UpdateItem(Expression<Func<object>> Expression, Expression<Func<T, bool>> primaryKey);
        Task<IQueryResult<bool>> DeleteItem(Expression<Func<T,bool>> whereLambda);

    }
}
