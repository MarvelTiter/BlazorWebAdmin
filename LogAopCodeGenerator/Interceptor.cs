using System.Text;
using System.Threading.Tasks;

namespace LogAopCodeGenerator
{
    public abstract class Interceptor
    {
        public abstract Task Invoke(AspectContext context);
    }
}
