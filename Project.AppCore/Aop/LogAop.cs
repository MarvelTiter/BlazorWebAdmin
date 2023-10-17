using LogAopCodeGenerator;
using Project.AppCore.Services;
using Project.AppCore.Store;
using Project.Models;
using Project.Models.Entities;
using Project.Models.Permissions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Project.AppCore.Aop
{
    public class LogAop : Interceptor
    {
        private readonly IRunLogService logService;
        private readonly UserStore store;

        public LogAop(IRunLogService logService, UserStore store)
        {
            this.logService = logService;
            this.store = store;
        }
               
        public override async Task Invoke(AspectContext context)
        {
            await context.Proceed();
            var infoAttr = context.ServiceMethod.GetCustomAttribute<LogInfoAttribute>();
            var result = context.ReturnValue as IQueryResult;
            var userId = store?.UserId ?? GetUserIdFromContext(context);
            if (infoAttr!.Module == "BasicService")
            {
                var type = context.ServiceType.GetGenericArguments().First();
                infoAttr!.Action = $"[{type.Name}]{infoAttr!.Action}";
            }
            var l = new RunLog()
            {
                UserId = userId,
                ActionModule = infoAttr!.Module ?? "",
                ActionName = infoAttr!.Action ?? "",
                ActionResult = result?.Success ?? false ? "成功" : "失败",
                ActionMessage = result?.Message ?? "",
            };
            await logService.Log(l);
        }

        private string GetUserIdFromContext(AspectContext context)
        {
            if (context.ServiceType == typeof(ILoginService))
            {
                // UpdateLastLoginTimeAsync 和 LoginAsync
                return (context.Parameters.FirstOrDefault() as UserInfo)?.UserId ?? context.Parameters.FirstOrDefault()?.ToString();
            }
            return "Unknow";
        }
    }
}
