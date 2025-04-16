using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Project.Constraints.Utils;

public static class EnumHelper<TEnum>
{
    private static readonly TEnum[] values;
    private static readonly Type enumType = Nullable.GetUnderlyingType(typeof(TEnum)) ?? typeof(TEnum);
    static EnumHelper()
    {
        values = [.. Enum.GetValues(enumType).Cast<TEnum>()];
    }

    public static IEnumerable<TEnum> GetValues() => values;

    public static string? GetDisplayName(TEnum value)
    {
        var name = Enum.GetName(enumType, value!);
        if (string.IsNullOrEmpty(name)) return null;
        return enumType.GetField(name)?.GetCustomAttribute<DisplayAttribute>(true)?.GetName();
    }

    public static TEnum? Parse(string value)
    {
        if (Enum.TryParse(enumType, value, out var enumValue))
        {
            return (TEnum)enumValue;
        }
        return default;
    }
    public static Dictionary<string, string> ParseDictionary()
    {
        Dictionary<string, string> dict = [];
        foreach (var member in Enum.GetValues(enumType).Cast<Enum>())
        {
            var name = Enum.GetName(enumType, member)!;
            var mem = enumType.GetMember(name)[0];
            var label = mem.GetCustomAttribute<DisplayAttribute>()?.Name ?? name;
            dict.Add(name, label!);
        }
        return dict;
    }
}