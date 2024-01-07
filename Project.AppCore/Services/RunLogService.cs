using MDbContext.ExpressionSql;
using MDbContext.Repository;

namespace Project.AppCore.Services
{
    public class RunLogService : IRunLogService
    {
        private readonly IExpressionContext context;

        public RunLogService(IExpressionContext context)
        {
            this.context = context;
        }

        public async Task<IQueryCollectionResult<RunLog>> GetRunLogsAsync(GenericRequest<RunLog> req)
        {
            var list = await context.Repository<RunLog>().GetListAsync(req.Expression, out var total, req.PageIndex, req.PageSize, log => log.LogId, false);
            return list.CollectionResult((int)total);
        }

        public async Task Log(RunLog log)
        {
            await context.Repository<RunLog>().InsertAsync(log);
        }
    }
}
