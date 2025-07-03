using System.Text.Json;

namespace Project.Web.Shared.Locales.Services;

public class JsonInfo
{
    public JsonDocument? Fallback { get; set; }
    public JsonDocument? Main { get; set; }
    public bool UseTypedName { get; set; }
    public string? SearchedLocation { get; set; }
    private bool TryGetValue(JsonElement? root, string key, string typedName, out string? value)
    {
        if (!root.HasValue)
        {
            value = null;
            return false;
        }
        var node = root.Value;
        if (UseTypedName && node.TryGetProperty(typedName, out var n1))
        {
            node = n1;
        }
        if (key.IndexOf('.') > -1)
        {
            var paths = new Queue<string>(key.Split('.'));
            while (paths.Count > 0)
            {
                var p = paths.Dequeue();
                if (p == typedName && UseTypedName)
                {
                    if (node.TryGetProperty(p, out var n2))
                    {
                        node = n2;
                    }
                    continue;
                }
                if (!node.TryGetProperty(p, out node))
                {
                    value = null;
                    return false;
                }
            }
            value = node.GetString();
            return true;
        }
        else
        {
            if (node.TryGetProperty(key, out node) && node.ValueKind == JsonValueKind.String)
            {
                value = node.GetString();
                return true;
            }
            value = null;
            return false;
        }
    }

    public bool TryGetValueFromMain(string key, string typedName, out string? value)
    {
        return TryGetValue(Main?.RootElement, key, typedName, out value);
    }

    public bool TryGetValueFromFallback(string key, string typedName, out string? value)
    {
        return TryGetValue(Fallback?.RootElement, key, typedName, out value);
    }
}