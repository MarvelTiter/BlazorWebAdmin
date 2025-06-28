using Project.Constraints.Aop;
using Project.Constraints.Models;
using Project.Constraints.Models.Request;
using System.Linq.Expressions;

namespace Project.Constraints.Services;

//   //[Aspectable(AspectHandleType = typeof(LogAop))]
//   [LogAop]
//public partial interface IBasicService<T>
//{
//	//[LogInfo(Action = "查询", Module = "BasicService")]
//	Task<IQueryCollectionResult<T>> GetListAsync(GenericRequest<T> req);
//	Task<IQueryResult<T>> GetSingleAsync(Expression<Func<T, bool>> whereExp);
//	[LogInfo(Action = "新增", Module = "BasicService")]
//	Task<IQueryResult<bool>> AddItem(T item);
//	[LogInfo(Action = "更新", Module = "BasicService")]
//	Task<IQueryResult<bool>> UpdateItem(T item, Expression<Func<T, bool>> primaryKey);
//	[LogInfo(Action = "更新", Module = "BasicService")]
//	Task<IQueryResult<bool>> UpdateItem(Expression<Func<object>> exp, Expression<Func<T, bool>> primaryKey);
//	[LogInfo(Action = "删除", Module = "BasicService")]
//	Task<IQueryResult<bool>> DeleteItem(Expression<Func<T, bool>> whereLambda);
//}