using AutoWasmApiGenerator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Constraints.Services;
public record SvgParsingResult(string InnerContent, Dictionary<string, object> Attributes, string OriginalContent);

[WebController(Route = "svg")]
public interface ISvgIconService
{
    [WebMethod(Method = WebMethod.Get)]
    Task<QueryResult<SvgParsingResult>> GetIconAsync(string? name);
    Task<QueryCollectionResult<string>> GetAllIcon();
}
[WebController(Route = "file")]
public interface IFileService
{
    [ApiInvokeNotSupported]
    string GetStaticFileWithVersion(string path);
    Task<string> GetStaticFileWithVersionAsync(string path);
}