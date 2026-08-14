using AutoWasmApiGenerator;

namespace BlazorTemplate.ClientCore.Services;

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


[WebController(Route = "template/runlog", Authorize = true)]
public interface ITemplateRunLogService : IRunLogService<TemplateRunLog> { }
