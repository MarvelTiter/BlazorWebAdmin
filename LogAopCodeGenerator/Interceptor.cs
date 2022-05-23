using System.Text;
using System.Threading.Tasks;

namespace LogAopCodeGenerator
{
    public abstract class Interceptor
    {
        public abstract Task<bool> Before(AspectContext context);
        public abstract Task After(AspectContext context);        
    }
}
