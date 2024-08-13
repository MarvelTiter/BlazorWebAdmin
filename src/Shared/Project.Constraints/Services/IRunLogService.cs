using AutoInjectGenerator;
using AutoWasmApiGenerator;
using MT.Generators.Abstraction;
using Project.Constraints.Models;
using Project.Constraints.Models.Permissions;
using Project.Constraints.Models.Request;

namespace Project.Constraints.Services
{
    [WebController(Route = "runlog", Authorize = true)]
    [ApiInvokerGenerate(typeof(AutoInjectAttribute))]
    [AttachAttributeArgument(typeof(ApiInvokerGenerateAttribute), typeof(AutoInjectAttribute), "Group", "WASM")]
    public interface IStandardRunLogService : IRunLogService<RunLog> { }
    public interface IRunLogService<TRunLog> where TRunLog : IRunLog
    {
        Task<QueryCollectionResult<TRunLog>> GetRunLogsAsync(GenericRequest<TRunLog> runLog);
        Task<QueryResult> WriteLog(TRunLog log);
    }

    public interface IRunLogService
    {
        Task<QueryResult> WriteLog<T>(T log) where T: IRunLog;
    }
}
