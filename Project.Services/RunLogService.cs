using Project.AppCore.Repositories;
using Project.AppCore.Services;
using Project.Models;
using Project.Models.Entities;
using Project.Models.Request;

namespace Project.Services
{
    public class RunLogService : IRunLogService
    {
        private readonly IRepository repository;

        public RunLogService(IRepository repository)
        {
            this.repository = repository;
        }

        public async Task<IQueryCollectionResult<RunLog>> GetRunLogsAsync(GenericRequest<RunLog> req)
        {
            var total =await repository.Table<RunLog>().GetCountAsync(req.Expression);
            var list = await repository.Table<RunLog>().GetListAsync(req.Expression,req.PageIndex, req.PageSize, log => log.LogId, false);
            return QueryResult.Success<RunLog>().CollectionResult(list, total);
        }

        public async Task Log(RunLog log)
        {
            await repository.Table<RunLog>().InsertAsync(log);
        }
    }
}
