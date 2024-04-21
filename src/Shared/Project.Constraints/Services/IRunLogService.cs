using Project.Constraints.Models;
using Project.Constraints.Models.Permissions;
using Project.Constraints.Models.Request;

namespace Project.Constraints.Services
{
    [AutoInject]
    public interface IRunLogService<TRunLog> : IRunLogService where TRunLog : IRunLog
    {
        Task<IQueryCollectionResult<TRunLog>> GetRunLogsAsync(GenericRequest<TRunLog> runLog);
        Task WriteLog(TRunLog log);
    }

    public interface IRunLogService
    {
        Task Log(IRunLog log);
    }
}
