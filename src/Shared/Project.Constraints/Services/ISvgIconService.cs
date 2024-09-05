using AutoWasmApiGenerator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Constraints.Services
{
    [WebController(Route = "svg", Authorize = true)]
    [ApiInvokerGenerate]
    public interface ISvgIconService
    {
        Task<QueryResult<string>> GetIcon(string? name);
        Task<QueryCollectionResult<string>> GetAllIcon();
    }
}
