using AspectCore.DynamicProxy;
using Project.Common.Attributes;
using System.Reflection;
namespace BlazorWebAdmin.Aop
{
    public class LogAop : AbstractInterceptor
    {
        public LogAop(IRunLogService)
        {

        }
        public override async Task Invoke(AspectContext context, AspectDelegate next)
        {
            await next(context);
            var infoAttr = context.ServiceMethod.GetCustomAttribute<LogInfoAttribute>();
            if (infoAttr == null) return;
            var result = context.ReturnValue as dynamic;
            Console.WriteLine($"{result?.Result.Success} {result?.Result.Message} {infoAttr?.Module} {infoAttr?.Action}");
        }
    }
}
