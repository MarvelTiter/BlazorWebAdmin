using Project.Constraints.Models;
using Project.Constraints.Models.Permissions;
using Project.Constraints.Models.Request;

namespace Project.Constraints.Services
{
    [AutoInject]
    public interface IRunLogService
    {
        Task<IQueryCollectionResult<RunLog>> GetRunLogsAsync(GenericRequest<RunLog> runLog);
        Task Log(RunLog log);
    }
}
