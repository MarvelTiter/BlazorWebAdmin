using Project.Models;
using Project.Models.Entities;
using Project.Models.Request;

namespace Project.Constraints.Services
{
	public interface IRunLogService
	{
		Task<IQueryCollectionResult<RunLog>> GetRunLogsAsync(GenericRequest<RunLog> runLog);
		Task Log(RunLog log);
	}
}
