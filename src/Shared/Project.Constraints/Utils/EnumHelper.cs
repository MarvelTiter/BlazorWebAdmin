using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Project.Constraints.Utils;

public static class EnumHelper<TEnum>
{
    private static readonly IEnumerable<TEnum> values;
    private static readonly Type enumType = Nullable.GetUnderlyingType(typeof(TEnum)) ?? typeof(TEnum);
    static EnumHelper()
    {
        values = Enum.GetValues(enumType).Cast<TEnum>();
    }

    public static IEnumerable<TEnum> GetValues() => values;

    public static string GetDisplayName(TEnum value)
    {
        string name = Enum.GetName(enumType, value);
        return enumType.GetField(name).GetCustomAttribute<DisplayAttribute>(true)?.GetName() ?? name;
    }

    public static TEnum? Parse(string value)
    {
        if (Enum.TryParse(enumType, value, out var enumValue))
        {
            return (TEnum)enumValue;
        }
        return default;
    }
}