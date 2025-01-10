using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace Project.Web.Shared.Utils;

public static partial class FontAwesomeHelper
{
    static readonly string[] icons = [];
    static FontAwesomeHelper()
    {
        var assembly = typeof(FontAwesomeHelper).Assembly;
        var resourceName = $"Project.Web.Shared.wwwroot.css.font-awesome4.7.min.css";
        var resource = assembly.GetManifestResourceStream(resourceName);
        if (resource is null) return;
        using var reader = new StreamReader(resource);
        var content = reader.ReadToEnd();
        var regex = Extrac47IconName();
        icons = [.. regex.Matches(content).Select(m => $"fa fa-{m.Groups[1].Value}")];
    }

   
    public static string[] AllIcons() => icons;

    //[GeneratedRegex("fa-(.+):before")]
    [GeneratedRegex("\\.fa-([a-zA-Z0-9-]+):before")]
    public static partial Regex Extrac47IconName();
}