using AspectCore.Configuration;
using AspectCore.DynamicProxy;
namespace BlazorWebAdmin.Aop
{   
    public class LogAopFactory : InterceptorFactory
    {
        public override IInterceptor CreateInstance(IServiceProvider serviceProvider)
        {
            Console.WriteLine($"CustomFactory {serviceProvider.GetType().FullName} {serviceProvider.GetHashCode()} ");
            return serviceProvider.GetService<LogAop>()!;
        }
    }
}
