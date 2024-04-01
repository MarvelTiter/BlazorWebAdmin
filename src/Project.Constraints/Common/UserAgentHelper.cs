using MT.Toolkit.ReflectionExtension;
using System.Text.RegularExpressions;

namespace Project.Constraints.Common;
public struct BrowserInfo(string? name, string? version, string? core)
{
    public string? Name { get; } = name;
    public string? Version { get; } = version;
    public string? Core { get; } = core;
    public readonly bool GreaterThen(int major)
    {
        if (Version == null) return false;
        var firstDot = Version.IndexOf('.');
        return int.TryParse(Version[..firstDot], out var majorVersion) && majorVersion >= major;
    }
}
/// <summary>
/// https://github.com/mycsharp/HttpUserAgentParser
/// </summary>
public static class UserAgentHelper
{
    private const RegexOptions DefaultBrowserRegexFlags = RegexOptions.IgnoreCase | RegexOptions.Compiled;
    private static Regex CreateDefaultBrowserRegex(string key) => new($@"{key}.*?([0-9\.]+)", DefaultBrowserRegexFlags);
    public static readonly string[] SupportedBrowsers = ["Edge", "Chrome", "Firefox", "Mozilla", "360", "360SE"];
    public static readonly string[] SupportedCores = ["WebKit", "Gecko", "KHTML"];
    private static readonly Dictionary<Regex, string> Browsers = new()
        {
            { CreateDefaultBrowserRegex("OPR"), "Opera" },
            { CreateDefaultBrowserRegex("Edge"), "Edge" },
            { CreateDefaultBrowserRegex("EdgA"), "Edge" },
            { CreateDefaultBrowserRegex("Edg"), "Edge" },
            { CreateDefaultBrowserRegex("Brave Chrome"), "Brave" },
            { CreateDefaultBrowserRegex("Chrome"), "Chrome" },
            { CreateDefaultBrowserRegex("CriOS"), "Chrome" },
            { CreateDefaultBrowserRegex("Opera.*?Version"), "Opera" },
            { CreateDefaultBrowserRegex("Opera"), "Opera" },
            { CreateDefaultBrowserRegex("MSIE"), "Internet Explorer" },
            { CreateDefaultBrowserRegex("Internet Explorer"), "Internet Explorer" },
            { CreateDefaultBrowserRegex("Trident.* rv"), "Internet Explorer" },
            { CreateDefaultBrowserRegex("Firefox"), "Firefox" },
            { CreateDefaultBrowserRegex("FxiOS"), "Firefox" },
            { CreateDefaultBrowserRegex("Netscape"), "Netscape" },
            { CreateDefaultBrowserRegex("OmniWeb"), "OmniWeb" },
            { CreateDefaultBrowserRegex("Version"), "Safari" }, // https://github.com/mycsharp/HttpUserAgentParser/issues/34
            { CreateDefaultBrowserRegex("Mozilla"), "Mozilla" },
            { CreateDefaultBrowserRegex("Konqueror"), "Konqueror" },
            { CreateDefaultBrowserRegex("Maxthon"), "Maxthon" },
            { CreateDefaultBrowserRegex("ipod touch"), "Apple iPod" },
            // 360
            { CreateDefaultBrowserRegex("Qihoo"), "360" },
            { CreateDefaultBrowserRegex("QH"), "360" },
            { CreateDefaultBrowserRegex("360EE"), "360EE" },
            { CreateDefaultBrowserRegex("360SE"), "360SE" },
            // UC
            { CreateDefaultBrowserRegex("UCBrowser"), "UC" },
            { CreateDefaultBrowserRegex("UBrowser"), "UC" },
            { CreateDefaultBrowserRegex("UCWEB"), "UC" },
            // QQ
            { CreateDefaultBrowserRegex("QQBrowser"), "QQBrowser" },
            // sogou
            { CreateDefaultBrowserRegex("MetaSr"), "Sogou" },
            { CreateDefaultBrowserRegex("Sogou"), "Sogou" },
            // Liebao
            { CreateDefaultBrowserRegex("LBBROWSER"), "Liebao" },
            { CreateDefaultBrowserRegex("LieBaoFast"), "Liebao" },
        };

    private static readonly Dictionary<Regex, string> Cores = new()
        {
            { CreateDefaultBrowserRegex("Trident"), "Trident" },
            { CreateDefaultBrowserRegex("NET CLR"), "Trident" },
            { CreateDefaultBrowserRegex("Presto"), "Presto" },
            { CreateDefaultBrowserRegex("AppleWebKit"), "WebKit" },
            { CreateDefaultBrowserRegex("Gecko/"), "Gecko" },
            { CreateDefaultBrowserRegex("KHTML/"), "KHTML" },
        };

    public static bool IsSupport(this BrowserInfo info, int majorVersion)
    {
        var clientTypeEnable = SupportedCores.Any(s => s == info.Core) || SupportedBrowsers.Any(s => s == info.Name);
        var versionEnable = info.Core == "WebKit" ? info.GreaterThen(majorVersion) : true;

        return clientTypeEnable && versionEnable;
    }

    public static BrowserInfo GetBrowser(string userAgent)
    {
        string? name = null, version = null, core = null;
        foreach ((Regex key, string? value) in Browsers)
        {
            Match match = key.Match(userAgent);
            if (match.Success)
            {
                name = value;
                version = match.Groups[1].Value;
                break;
            }
        }
        foreach ((Regex key, string? value) in Cores)
        {
            Match match = key.Match(userAgent);
            if (match.Success)
            {
                core = value;
                break;
            }
        }
        return new BrowserInfo(name, version, core);
    }
}
