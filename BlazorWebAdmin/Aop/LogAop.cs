using AspectCore.DynamicProxy;
using BlazorWebAdmin.Store;
using Microsoft.AspNetCore.Components.Authorization;
using Project.Common.Attributes;
using Project.Models.Entities;
using Project.Services.interfaces;
using System.Reflection;
namespace BlazorWebAdmin.Aop
{
    public class LogAop : AbstractInterceptor
    {
        private readonly IRunLogService logService;
		private readonly UserStore store;

		public LogAop(IRunLogService logService, UserStore store)
        {
            this.logService = logService;
			this.store = store;
			Console.WriteLine($"LogAop store {store.GetHashCode()}");
		}
        public override async Task Invoke(AspectContext context, AspectDelegate next)
        {
            await next(context);
            var infoAttr = context.ServiceMethod.GetCustomAttribute<LogInfoAttribute>();
            if (infoAttr == null) return;
            var result = context.ReturnValue as dynamic;
            //var msg = $"{result?.Result.Success} {result?.Result.Message} {infoAttr?.Module} {infoAttr?.Action}";
            var userId = GetUserId(context) ?? "获取失败";
            var l = new RunLog()
            {
                UserId = userId,
                ActionModule = infoAttr!.Module ?? "",
                ActionName = infoAttr!.Action ?? "",
                ActionResult = result?.Result.Success ? "成功" : "失败",
                ActionMessage = result?.Result.Message ?? "",
            };
            await logService.Log(l);
        }

        string? GetUserId(AspectContext context)
        {
            if (context.ServiceMethod.Name == "LoginAsync")
            {
                return context.Parameters.FirstOrDefault()?.ToString();
            }
            return store.UserId;
        }
    }
}
