using System;
using System.Collections.Generic;

namespace SvgFileParse;

public class Utils
{

    public static string GetRawStringLiteral(string text)
    {
        string delimiter = "\"\"\"";
        return $@"
{delimiter}
{text.Trim('\r', '\n')}
{delimiter}";
    }

    public static SvgParsingResult ProcessSvgContent(string content)
    {
        content = CleanSvgContent(content);
        return ParseSvgContent(content);
    }

    private static SvgParsingResult ParseSvgContent(string content)
    {
        // 查找<svg>标签
        int svgStart = content.IndexOf("<svg", StringComparison.OrdinalIgnoreCase);
        if (svgStart == -1)
        {
            return new SvgParsingResult(content, new Dictionary<string, object>(), content);
        }

        // 查找<svg>标签结束位置
        int svgTagEnd = content.IndexOf('>', svgStart);
        if (svgTagEnd == -1)
        {
            return new SvgParsingResult(content, new Dictionary<string, object>(), content);
        }

        int svgContentStart = svgTagEnd + 1;

        // 提取属性
        var attributes = ExtractSvgAttributes(content, svgStart, svgTagEnd);

        // 查找</svg>标签
        int svgEnd = content.IndexOf("</svg>", svgContentStart, StringComparison.OrdinalIgnoreCase);
        string innerContent;

        if (svgEnd == -1)
        {
            innerContent = content.Substring(svgContentStart);
        }
        else
        {
            innerContent = content.Substring(svgContentStart, svgEnd - svgContentStart);
        }

        return new SvgParsingResult(innerContent, attributes, content);
    }

    private static string CleanSvgContent(string content)
    {
        content = RemoveXmlDeclaration(content);
        content = RemoveDoctype(content);
        return content.Trim();
    }

    private static string RemoveXmlDeclaration(string content)
    {
        if (content.StartsWith("<?xml", StringComparison.OrdinalIgnoreCase))
        {
            int end = content.IndexOf("?>", StringComparison.Ordinal);
            if (end != -1)
            {
                return content.Substring(end + 2).Trim();
            }
        }
        return content;
    }

    private static string RemoveDoctype(string content)
    {
        if (content.StartsWith("<!DOCTYPE", StringComparison.OrdinalIgnoreCase))
        {
            int end = content.IndexOf('>');
            if (end != -1)
            {
                return content.Substring(end + 1).Trim();
            }
        }
        return content;
    }

    private static Dictionary<string, object> ExtractSvgAttributes(string svgTag, int start, int end)
    {
        var attributes = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

        // 跳过"<svg"
        int currentPos = start + 4;

        while (currentPos < end)
        {
            // 跳过空白字符
            while (currentPos < end && char.IsWhiteSpace(svgTag[currentPos]))
            {
                currentPos++;
            }

            if (currentPos >= end) break;

            // 查找属性名
            int nameStart = currentPos;
            while (currentPos < end && !char.IsWhiteSpace(svgTag[currentPos]) &&
                   svgTag[currentPos] != '=' && svgTag[currentPos] != '>')
            {
                currentPos++;
            }

            if (currentPos >= end) break;

            string name = svgTag.Substring(nameStart, currentPos - nameStart);
            if (string.IsNullOrEmpty(name)) break;

            // 跳过等号
            while (currentPos < end && char.IsWhiteSpace(svgTag[currentPos]))
            {
                currentPos++;
            }

            if (currentPos >= end || svgTag[currentPos] != '=')
            {
                // 没有值的属性（如 disabled）
                attributes[name] = string.Empty;
                continue;
            }

            currentPos++; // 跳过'='

            // 跳过等号后的空白
            while (currentPos < end && char.IsWhiteSpace(svgTag[currentPos]))
            {
                currentPos++;
            }

            if (currentPos >= end) break;

            // 获取属性值
            char quoteChar = svgTag[currentPos];
            bool isQuoted = quoteChar == '"' || quoteChar == '\'';

            int valueStart = isQuoted ? currentPos + 1 : currentPos;
            int valueEnd;

            if (isQuoted)
            {
                valueEnd = svgTag.IndexOf(quoteChar, valueStart);
                if (valueEnd == -1) break;
            }
            else
            {
                valueEnd = valueStart;
                while (valueEnd < end && !char.IsWhiteSpace(svgTag[valueEnd]) &&
                       svgTag[valueEnd] != '>')
                {
                    valueEnd++;
                }
            }

            if (valueEnd >= end) break;

            string value = isQuoted ?
                svgTag.Substring(valueStart, valueEnd - valueStart) :
                svgTag.Substring(valueStart, valueEnd - valueStart);

            attributes[name] = value;

            currentPos = isQuoted ? valueEnd + 1 : valueEnd;
        }

        return attributes;
    }
}