using Project.Constraints.Models.Permissions;

namespace Project.AppCore.Services
{
    [IgnoreAutoInject]
    public class RunLogService<TRunLog> : IRunLogService<TRunLog> where TRunLog : class, IRunLog, new()
    {
        private readonly IExpressionContext context;

        public RunLogService(IExpressionContext context)
        {
            this.context = context;
        }

        public async Task<IQueryCollectionResult<TRunLog>> GetRunLogsAsync(GenericRequest<TRunLog> req)
        {
            var list = await context.Repository<TRunLog>().GetListAsync(req.Expression, out var total, req.PageIndex, req.PageSize, log => log.LogId, false);
            return list.CollectionResult((int)total);
        }

        public async Task WriteLog(TRunLog log)
        {
            await context.Repository<TRunLog>().InsertAsync(log);
        }

        public Task Log(IRunLog log)
        {
            var tlog = new TRunLog
            {
                UserId = log.UserId,
                ActionModule = log.ActionModule,
                ActionName = log.ActionName,
                ActionResult = log.ActionResult,
                ActionMessage = log.ActionMessage,
                ActionTime = log.ActionTime,
            };
            return WriteLog(tlog);
        }
    }


    public interface IStandardRunLogService : IRunLogService<RunLog> { }
    [WebApiGenerator.Attributes.WebController(Route = "runlog")]
    public class StandardRunLogService : RunLogService<RunLog>, IStandardRunLogService
    {
        public StandardRunLogService(IExpressionContext context) : base(context)
        {
        }
    }
}
