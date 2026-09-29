using System.Collections.Generic;

namespace SvgFileParse;

public class SvgParsingResult(string innerContent, Dictionary<string, object> attributes, string originalContent)
{
    public string InnerContent { get; } = innerContent;
    public Dictionary<string, object> Attributes { get; } = attributes;
    public string OriginalContent { get; } = originalContent;
}