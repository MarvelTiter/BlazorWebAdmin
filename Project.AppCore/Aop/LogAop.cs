using LogAopCodeGenerator;
using Project.AppCore.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.AppCore.Aop
{
    public class LogAop : Interceptor
    {
        private readonly UserStore store;

        public LogAop(UserStore store)
        {
            this.store = store;
        }
        public override Task After(AspectContext context)
        {
            Console.WriteLine($"from LogAop After {context.ServiceMethod.Name} User:{store?.UserId}");
            return Task.CompletedTask;
        }

        public override Task<bool> Before(AspectContext context)
        {
            Console.WriteLine($"from LogAop Before{context.ServiceMethod.Name} User:{store?.UserId}");
            return Task.FromResult(true);
        }
    }
}
