using AutoInjectGenerator;
using AutoWasmApiGenerator;
using MT.Generators.Abstraction;
using Project.Constraints.Models.Permissions;

namespace Project.AppCore.Services
{
    public class RunLogService<TRunLog> : IRunLogService<TRunLog> where TRunLog : class, IRunLog, new()
    {
        private readonly IExpressionContext context;

        public RunLogService(IExpressionContext context)
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

    [AutoInject(Group = "SERVER")]
    public class RunLogService : IRunLogService
    {
        private readonly IExpressionContext context;

        public RunLogService(IExpressionContext context)
        {
            this.context = context;
        }

        public async Task<QueryResult> WriteLog<T>(T log) where T : IRunLog
        {
            var i = await context.Repository<T>().InsertAsync(log);
            return (i > 0).Result();
        }
    }

    //[WebController(Route = "runlog")]
    [ApiInvokerGenera(typeof(AutoInjectAttribute))]
    [AttachAttributeArgument(typeof(ApiInvokerGeneraAttribute), typeof(AutoInjectAttribute), "Group", "WASM")]
    [AutoInject(Group = "SERVER", ServiceType = typeof(IStandardRunLogService))]
    public class StandardRunLogService : RunLogService<RunLog>, IStandardRunLogService
    {
        public StandardRunLogService(IExpressionContext context) : base(context)
        {
        }
    }
}
