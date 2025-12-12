using AutoAopProxyGenerator;
using AutoWasmApiGenerator;
using Project.Constraints.Aop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Constraints.Services;

[WebController(Route = "console", Authorize = true)]
[AddAspectHandler(AspectType = typeof(AopLogger))]
[AddAspectHandler(AspectType = typeof(AopPermissionCheck))]
public interface IConsoleOutputService
{
    Task<string> GetConsoleOutputAsync();
}
