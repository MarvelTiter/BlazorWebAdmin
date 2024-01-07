using Project.Constraints.Models;
using Project.Constraints.Models.Request;
using Project.Models.Entities;

namespace Project.Constraints.Services
{
    [AutoInject]
    public interface IRunLogService
    {
        Task<IQueryCollectionResult<RunLog>> GetRunLogsAsync(GenericRequest<RunLog> runLog);
        Task Log(RunLog log);
    }
}
