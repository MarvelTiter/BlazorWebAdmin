using System.Data;

namespace Project.Constraints.Common;

public static class ObjectExtensions
{
    public static T? ConvertTo<T>(this object value, object? defaultValue = null) => (T?)ConvertTo(typeof(T), value, defaultValue);

    public static object? ConvertTo(Type type, object value, object? defaultValue = null)
    {
        if (value == null || value == DBNull.Value)
            return defaultValue;

        var valueString = value.ToString()!;
        if (type == typeof(string))
            return Convert.ChangeType(valueString, type);

        valueString = valueString.Trim();
        if (valueString.Length == 0)
            return defaultValue;

        if (type.IsEnum)
            return Enum.Parse(type, valueString, true);

        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            type = Nullable.GetUnderlyingType(type)!;

        if (type == typeof(bool) || type == typeof(bool?))
            valueString = ",是,1,Y,YES,TRUE,".Contains(valueString.ToUpper()) ? "True" : "False";

        try
        {
            return Convert.ChangeType(valueString, type);
        }
        catch
        {
            return defaultValue;
        }
    }

    public static IEnumerable<DataRow> ToEnumerable(this DataTable table)
    {
        foreach (DataRow row in table.Rows)
        {
            yield return row;
        }
    }
}