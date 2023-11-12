using Project.AppCore.Aop;
using Project.Models;
using Project.Models.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Project.AppCore.Services
{
    //[Aspectable(AspectHandleType = typeof(LogAop))]
    [LogAop]
    public partial interface IBasicService<T>
    {
        //[LogInfo(Action = "查询", Module = "BasicService")]
        Task<IQueryCollectionResult<T>> GetListAsync(GenericRequest<T> req);
        Task<IQueryResult<T>> GetSingleAsync(Expression<Func<T, bool>> whereExp);
        [LogInfo(Action = "新增", Module = "BasicService")]
        Task<IQueryResult<bool>> AddItem(T item);
        [LogInfo(Action = "更新", Module = "BasicService")]
        Task<IQueryResult<bool>> UpdateItem(T item, Expression<Func<T, bool>> primaryKey);
        [LogInfo(Action = "更新", Module = "BasicService")]
        Task<IQueryResult<bool>> UpdateItem(Expression<Func<object>> exp, Expression<Func<T, bool>> primaryKey);
        [LogInfo(Action = "删除", Module = "BasicService")]
        Task<IQueryResult<bool>> DeleteItem(Expression<Func<T, bool>> whereLambda);
    }
}
