using AutoWasmApiGenerator;
using BlazorTemplate.Constraints.Models.Permissions;
using BlazorTemplate.Constraints.Models.Request;

namespace BlazorTemplate.Constraints.Services;

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
public interface IStandardRunLogService : IRunLogService<RunLog> { }
#endif