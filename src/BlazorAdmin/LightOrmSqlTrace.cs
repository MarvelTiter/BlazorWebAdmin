using LightORM.Implements;
using LightORM.Models;

namespace BlazorAdmin;

public class LightOrmSqlTrace(ILogger<LightOrmSqlTrace> logger) : AdoInterceptorBase
{
    public override void AfterExecute(SqlExecuteContext context)
    {
        logger.LogInformation("{TraceId}: 语句 -> {NewLine}{Sql}", context.TraceId, Environment.NewLine, context.Sql);
        logger.LogInformation("{TraceId}: 耗时 -> {Elapsed}", context.TraceId, context.Elapsed);
    }
    public override void OnException(SqlExecuteExceptionContext context)
    {
        base.OnException(context);
    }
}