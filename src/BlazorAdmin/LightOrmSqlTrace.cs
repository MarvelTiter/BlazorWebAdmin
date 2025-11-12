using LightORM.Implements;
using LightORM.Models;
using Project.Constraints.Store;

namespace BlazorAdmin;

public class LightOrmSqlTrace(IUserStore userStore, ILogger<LightOrmSqlTrace> logger) : AdoInterceptorBase
{
    public override void AfterExecute(SqlExecuteContext context)
    {
        logger.LogInformation("用户:{UserId} {TraceId}: 语句 -> {NewLine}{Sql}", userStore.UserInfo?.UserId, context.TraceId, Environment.NewLine, context.Sql);
        logger.LogInformation("用户:{UserId} {TraceId}: 耗时 -> {Elapsed}", userStore.UserInfo?.UserId, context.TraceId, context.Elapsed);
    }
    public override void OnException(SqlExecuteExceptionContext context)
    {
        base.OnException(context);
    }
}