using Microsoft.Extensions.Hosting;
using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Project.Web.Shared.Services;
[AutoInject(Group = "SERVER", LifeTime = InjectLifeTime.Singleton)]
public class SvgIconService : ISvgIconService
{
    private readonly IHostEnvironment environment;
    private static readonly ConcurrentDictionary<string, SvgParsingResult> _contentCache = new();
    private static readonly ConcurrentDictionary<string, string> _nameCache = new();
    public SvgIconService(IHostEnvironment environment)
    {
        this.environment = environment;
    }
    public Task<QueryCollectionResult<string>> GetAllIcon()
    {
        var names = _nameCache.Keys;
        return Task.FromResult(names.CollectionResult());
    }
    public async Task<QueryResult<SvgParsingResult>> GetIconAsync(string? name)
    {
        if (string.IsNullOrEmpty(name))
        {
            return QueryResult.Return<SvgParsingResult>(false);
        }
        if (!_contentCache.TryGetValue(name, out var result))
        {
            result = await ReadIconData(name, environment);
            if (result is not null)
                _contentCache.TryAdd(name, result);
        }
        return result;
    }


    static bool loaded;

    //private static string IconFullName(string name) => name.StartsWith(AppConst.CUSTOM_SVG_PREFIX) ? name : $"{AppConst.CUSTOM_SVG_PREFIX}{name}";

    private static async Task<SvgParsingResult?> ReadIconData(string name, IHostEnvironment environment)
    {
        if (!_nameCache.TryGetValue(name, out var iconFile))
        {
            if (!loaded)
            {
                LoadAllIcons(environment);
                return await ReadIconData(name, environment);
            }
        }
        if (iconFile != null)
        {
            var svgContent = await ProcessSvgFile(iconFile);
            return svgContent;
        }
        else
        {
            return null;
        }
    }

    private static void LoadAllIcons(IHostEnvironment environment)
    {
        if (loaded) return;
        var path = AppDomain.CurrentDomain.BaseDirectory;
        if (environment.IsDevelopment())
        {
            path = new DirectoryInfo(path).Parent!.Parent!.Parent!.Parent!.FullName;
        }
        var files = Directory.EnumerateFiles(path, "*.svg", SearchOption.AllDirectories).Where(f => f.Contains("wwwroot"));
        //var files = Directory.EnumerateFiles(path, "*.svg", SearchOption.AllDirectories).Where(f => f.Contains("SvgAssets", StringComparison.CurrentCultureIgnoreCase));
        foreach (var f in files)
        {
            var name = Path.GetFileNameWithoutExtension(f);
            if (!name.StartsWith(AppConst.CUSTOM_SVG_PREFIX)) continue;
            _nameCache.TryAdd(name, f);
        }
        loaded = true;
    }

    private static async ValueTask<SvgParsingResult?> ProcessSvgFile(string filePath)
    {
        byte[] fileBytes = await File.ReadAllBytesAsync(filePath);

        // 使用ArrayPool减少内存分配
        var charBuffer = ArrayPool<char>.Shared.Rent(Encoding.UTF8.GetMaxCharCount(fileBytes.Length));
        try
        {
            int charCount = Encoding.UTF8.GetChars(fileBytes, charBuffer);
            //ReadOnlySpan<char> contentSpan = charBuffer.AsSpan(0, charCount);

            return ParseSvgContent(charBuffer);
        }
        finally
        {
            ArrayPool<char>.Shared.Return(charBuffer);
        }
    }

    private static SvgParsingResult ParseSvgContent(ReadOnlySpan<char> content)
    {
        content = CleanSvgContent(content);
        // 查找<svg>标签
        int svgStart = content.IndexOf("<svg".AsSpan());
        if (svgStart == -1)
        {
            return new SvgParsingResult(content.ToString()
                , []
                , content.ToString());
        }

        // 查找<svg>标签结束位置
        int svgTagEnd = content[svgStart..].IndexOf('>');
        if (svgTagEnd == -1)
        {
            return 
                new SvgParsingResult(content.ToString()
                , []
                , content.ToString());
        }

        svgTagEnd += svgStart;
        int svgContentStart = svgTagEnd + 1;

        // 提取属性
        var attributes = ExtractSvgAttributes(content[svgStart..svgContentStart]);

        // 查找</svg>标签
        int svgEnd = content[svgContentStart..].IndexOf("</svg>".AsSpan());
        string innerContent;

        if (svgEnd == -1)
        {
            innerContent = content[svgContentStart..].ToString();
        }
        else
        {
            innerContent = content[svgContentStart..(svgContentStart + svgEnd)].ToString();
        }

        return new SvgParsingResult(innerContent, attributes, content.ToString());
    }

    private static ReadOnlySpan<char> CleanSvgContent(ReadOnlySpan<char> content)
    {
        // 移除XML声明
        content = RemoveXmlDeclaration(content);

        // 移除DOCTYPE声明
        content = RemoveDoctype(content);

        // 返回清理后的内容
        return content;
    }

    private static ReadOnlySpan<char> RemoveXmlDeclaration(ReadOnlySpan<char> content)
    {
        if (content.StartsWith("<?xml".AsSpan(), StringComparison.OrdinalIgnoreCase))
        {
            int end = content.IndexOf("?>".AsSpan());
            if (end != -1)
            {
                return content[(end + 2)..].Trim();
            }
        }
        return content;
    }

    private static ReadOnlySpan<char> RemoveDoctype(ReadOnlySpan<char> content)
    {
        if (content.StartsWith("<!DOCTYPE".AsSpan(), StringComparison.OrdinalIgnoreCase))
        {
            int end = content.IndexOf('>');
            if (end != -1)
            {
                return content[(end + 1)..].Trim();
            }
        }
        return content;
    }

    private static Dictionary<string, object> ExtractSvgAttributes(ReadOnlySpan<char> svgTag)
    {
        var attributes = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

        // 跳过"<svg"
        int currentPos = 4;

        while (currentPos < svgTag.Length)
        {
            // 跳过空白字符
            while (currentPos < svgTag.Length && char.IsWhiteSpace(svgTag[currentPos]))
            {
                currentPos++;
            }

            if (currentPos >= svgTag.Length) break;

            // 查找属性名
            int nameStart = currentPos;
            while (currentPos < svgTag.Length && !char.IsWhiteSpace(svgTag[currentPos]) &&
                   svgTag[currentPos] != '=' && svgTag[currentPos] != '>')
            {
                currentPos++;
            }

            if (currentPos >= svgTag.Length) break;

            var nameSpan = svgTag[nameStart..currentPos];
            if (nameSpan.IsEmpty) break;

            // 跳过等号
            while (currentPos < svgTag.Length && char.IsWhiteSpace(svgTag[currentPos]))
            {
                currentPos++;
            }

            if (currentPos >= svgTag.Length || svgTag[currentPos] != '=')
            {
                // 没有值的属性（如 disabled）
                attributes[nameSpan.ToString()] = string.Empty;
                continue;
            }

            currentPos++; // 跳过'='

            // 跳过等号后的空白
            while (currentPos < svgTag.Length && char.IsWhiteSpace(svgTag[currentPos]))
            {
                currentPos++;
            }

            if (currentPos >= svgTag.Length) break;

            // 获取属性值
            char quoteChar = svgTag[currentPos];
            bool isQuoted = quoteChar == '"' || quoteChar == '\'';

            int valueStart = isQuoted ? currentPos + 1 : currentPos;
            int valueEnd;

            if (isQuoted)
            {
                valueEnd = svgTag[valueStart..].IndexOf(quoteChar);
                if (valueEnd == -1) break;
                valueEnd += valueStart;
            }
            else
            {
                valueEnd = valueStart;
                while (valueEnd < svgTag.Length && !char.IsWhiteSpace(svgTag[valueEnd]) &&
                       svgTag[valueEnd] != '>')
                {
                    valueEnd++;
                }
            }

            if (valueEnd >= svgTag.Length) break;

            var valueSpan = isQuoted ?
                svgTag[valueStart..valueEnd] :
                svgTag[valueStart..valueEnd];

            attributes[nameSpan.ToString()] = valueSpan.ToString();

            currentPos = isQuoted ? valueEnd + 1 : valueEnd;
        }

        return attributes;
    }
}