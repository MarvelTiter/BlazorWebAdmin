using AspectCore.DynamicProxy;
using BlazorWebAdmin.Store;
using Project.Common.Attributes;
using Project.Models.Entities;
using Project.Services.interfaces;
using System.Reflection;
namespace BlazorWebAdmin.Aop
{
    public class LogAop : AbstractInterceptor
    {
        private readonly IRunLogService logService;
        private readonly ILogger logger;
        private readonly UserStore store;

        public LogAop(IRunLogService logService, ILogger<LogAop> logger, UserStore store)
        {
            this.logService = logService;
            this.logger = logger;
            this.store = store;
        }
        public override async Task Invoke(AspectContext context, AspectDelegate next)
        {
            await next(context);
            var infoAttr = context.ServiceMethod.GetCustomAttribute<LogInfoAttribute>();
            if (infoAttr == null) return;
            var result = context.ReturnValue as dynamic;
            var msg = $"{result?.Result.Success} {result?.Result.Message} {infoAttr?.Module} {infoAttr?.Action}";
            logger.LogInformation(msg);
            var l = new RunLog()
            {
                UserId = GetUserId(context.Parameters) ?? "获取失败",
                ActionModule = infoAttr!.Module ?? "",
                ActionName = infoAttr!.Action ?? "",
                ActionResult = result?.Result.Success ? "成功" : "失败",
                ActionMessage = result?.Result.Message ?? "",
            };
            await logService.Log(l);
        }

        string? GetUserId(object[] parameters)
        {
            if (store.UserId == null)
            {
                return parameters.FirstOrDefault()?.ToString();
            }
            return store.UserId;
        }
    }
}
