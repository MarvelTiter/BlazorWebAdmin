using AutoInjectGenerator;
using LightORM;

namespace Project.AppCore.Services
{
    public class DefaultRunLogService<TRunLog> where TRunLog : class, IRunLog, new()
    {
        private readonly IExpressionContext context;

        public DefaultRunLogService(IExpressionContext context)
        {
            this.context = context;
        }

        public async Task<QueryCollectionResult<TRunLog>> GetRunLogsAsync(GenericRequest<TRunLog> req)
        {
            var list = await context.Repository<TRunLog>().GetListAsync(req.Expression(), out var total, req.PageIndex, req.PageSize, log => log.LogId, false);
            return list.CollectionResult((int)total);
        }

        public async Task<QueryResult> WriteLog(TRunLog log)
        {
            var i = await context.Repository<TRunLog>().InsertAsync(log);
            return (i > 0).Result();
        }
    }

#if (ExcludeDefaultService)
#else
    [AutoInject(Group = "SERVER")]
    public class DefaultRunLogService : IRunLogService
    {
        private readonly IExpressionContext context;

        public DefaultRunLogService(IExpressionContext context)
        {
            this.context = context;
        }

        public async Task<QueryResult> WriteLog(MinimalLog log)
        {
            var l = new RunLog
            {
                UserId = log.UserId,
                ActionModule = log.Module,
                ActionName = log.Action,
                ActionResult = log.Result,
                ActionMessage = log.Message,
            };
            var i = await context.Repository<RunLog>().InsertAsync(l);
            return (i > 0).Result();
        }
    }


    [AutoInject(Group = "SERVER", ServiceType = typeof(IStandardRunLogService))]
    public class StandardRunLogService : DefaultRunLogService<RunLog>, IStandardRunLogService
    {
        public StandardRunLogService(IExpressionContext context) : base(context)
        {
        }
    }
#endif
}
