using AutoWasmApiGenerator;

namespace BlazorTemplate.ClientCore.Services;

public record SvgParsingResult(string Name, string InnerContent, Dictionary<string, object> Attributes, string OriginalContent);

public interface ISvgIconService
{
    //[WebMethod(Method = WebMethod.Get)]
    //Task<QueryResult<SvgParsingResult>> GetIconAsync(string? name);
    //Task<QueryCollectionResult<string>> GetAllIcon();
    SvgParsingResult? GetIcon(string? name);
    ICollection<SvgParsingResult> GetAllIcon();
    abstract static void RegisterIcons<T>() where T : ISvgIconProvider;
}

public interface ISvgIconProvider
{
    abstract static IEnumerable<SvgParsingResult> GetIcons();
}

[WebController(Route = "file")]
public interface IFileService
{
    [ApiClientNotSupported]
    string GetStaticFileWithVersion(string path);
    Task<string> GetStaticFileWithVersionAsync(string path);
}