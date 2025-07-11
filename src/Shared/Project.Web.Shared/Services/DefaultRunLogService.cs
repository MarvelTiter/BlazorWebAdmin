﻿using AutoInjectGenerator;
using LightORM;

namespace Project.AppCore.Services;

public class DefaultRunLogService<TRunLog> : IRunLogService where TRunLog : class, IRunLog, new()
{
    protected readonly IExpressionContext context;

    public DefaultRunLogService(IExpressionContext context)
    {
        this.context = context;
    }

    public async Task<QueryCollectionResult<TRunLog>> GetRunLogsAsync(GenericRequest<TRunLog> req)
    {
        var list = await context.Select<TRunLog>().Where(req.Expression())
            .Count(out var total)
            .Paging(req.PageIndex, req.PageSize)
            .OrderByDesc(log => log.LogId)
            .ToListAsync();
        return list.CollectionResult((int)total);
    }

    public async Task<QueryResult> WriteLog(TRunLog log)
    {
        var i = await context.Insert(log).ExecuteAsync();
        return i > 0;
    }
    async Task<QueryResult> IRunLogService.WriteLog(MinimalLog log)
    {
        var l = new TRunLog
        {
            UserId = log.UserId,
            ActionModule = log.Module,
            ActionName = log.Action,
            ActionResult = log.Result,
            ActionMessage = log.Message,
        };
        var i = await context.Insert(l).ExecuteAsync();
        return i > 0;
    }
}

#if (ExcludeDefaultService)
#else
//[AutoInject(Group = "SERVER")]
//public class DefaultRunLogService : IRunLogService
//{
//    private readonly IExpressionContext context;

//    public DefaultRunLogService(IExpressionContext context)
//    {
//        this.context = context;
//    }

//    public async Task<QueryResult> WriteLog(MinimalLog log)
//    {
//        var l = new RunLog
//        {
//            UserId = log.UserId,
//            ActionModule = log.Module,
//            ActionName = log.Action,
//            ActionResult = log.Result,
//            ActionMessage = log.Message,
//        };
//        var i = await context.Insert(l).ExecuteAsync();
//        return (i > 0).Result();
//    }
//}


[AutoInject(Group = "SERVER", ServiceType = typeof(IStandardRunLogService))]
[AutoInject(Group = "SERVER", ServiceType = typeof(IRunLogService))]
public class StandardRunLogService : DefaultRunLogService<RunLog>, IStandardRunLogService
{
    public StandardRunLogService(IExpressionContext context) : base(context)
    {
    }

}
#endif