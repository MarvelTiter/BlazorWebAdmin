using MDbContext.ExpressionSql;
using MDbContext.Repository;
using Project.AppCore.Repositories;
using Project.AppCore.Services;
using Project.Models;
using Project.Models.Entities;
using Project.Models.Request;

namespace Project.Services
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
            var list = await context.Repository<RunLog>().GetListAsync(req.Expression,out var total, req.PageIndex, req.PageSize, log => log.LogId, false);
            return list.Result((int)total);
        }

        public async Task Log(RunLog log)
        {
            await context.Repository<RunLog>().InsertAsync(log);
        }
    }
}
