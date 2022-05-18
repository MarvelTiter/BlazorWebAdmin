using AspectCore.Configuration;
using AspectCore.DynamicProxy;
namespace BlazorWebAdmin.Aop
{
    //public class LogAopAttribute : AbstractInterceptorAttribute
    //{
    //    public override async Task Invoke(AspectContext context, AspectDelegate next)
    //    {
    //        Console.WriteLine("Logging......");
    //        await next(context);
    //        var infoAttr = context.ServiceMethod.GetCustomAttribute<LogInfoAttribute>();
    //        if (infoAttr == null) return;
    //        var result = context.ReturnValue as dynamic;
    //        Console.WriteLine($"{result?.Result.Success} {result?.Result.Message} {infoAttr?.Module} {infoAttr?.Action}");
    //    }
    //}

    public class CustomFactory : InterceptorFactory
    {
        public override IInterceptor CreateInstance(IServiceProvider serviceProvider)
        {
            return (IInterceptor)serviceProvider.GetService(typeof(LogAop))!;
        }
    }
}
