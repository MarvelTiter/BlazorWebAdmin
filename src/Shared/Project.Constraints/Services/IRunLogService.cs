using AutoInjectGenerator;
using AutoWasmApiGenerator;
using MT.Generators.Abstraction;
using Project.Constraints.Models;
using Project.Constraints.Models.Permissions;
using Project.Constraints.Models.Request;

namespace Project.Constraints.Services
{
    public interface IRunLogService<TRunLog> : IRunLogService where TRunLog : IRunLog
    {
        Task<QueryCollectionResult<TRunLog>> GetRunLogsAsync(GenericRequest<TRunLog> runLog);
        Task<QueryResult> WriteLog(TRunLog log);
    }

    //[WebController(Route = "writelog", Authorize = true)]
    //[ApiInvokerGenerate]
    public interface IRunLogService
    {
        [WebMethod(Route = "write")]
        Task<QueryResult> WriteLog(MinimalLog log);
    }

#if (ExcludeDefaultService)
#else
    [WebController(Route = "runlog", Authorize = true)]
    [ApiInvokerGenerate]
    public interface IStandardRunLogService : IRunLogService<RunLog> { }
#endif
}
