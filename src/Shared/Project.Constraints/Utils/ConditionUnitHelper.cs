using Project.Constraints.Models.Request;
using Project.Constraints.UI;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json;

namespace Project.Constraints.Utils;

public static class ConditionUnitHelper
{
    public static Expression<Func<T, bool>> BuildExpression<T>(this ConditionUnit unit)
    {
        var parameterExpression = Expression.Parameter(typeof(T), "p");
        var body = SolveConditionUnit<T>(parameterExpression, unit.Children);
        if (body == null)
        {
            return t => true;
        }
        var lambda = Expression.Lambda<Func<T, bool>>(body, parameterExpression);
        return lambda;
    }

    public static Expression<Func<T, bool>> BuildTopExpression<T>(this ConditionUnit condition)
    {
        var parameterExpression = Expression.Parameter(typeof(T), "p");

        if (string.IsNullOrEmpty(condition.Name) || InvalidValue(condition.Value))
        {
            return t => true;
        }
        var body = BuildExpression<T>(parameterExpression, condition);
        if (body == null)
        {
            return t => true;
        }
        var lambda = Expression.Lambda<Func<T, bool>>(body, parameterExpression);
        return lambda;
    }

    private static Expression? SolveConditionUnit<T>(ParameterExpression pExp, IEnumerable<ConditionUnit> conditions)
    {
        Expression? returnExpression = null;
        foreach (var item in conditions)
        {
            if (string.IsNullOrEmpty(item.Name) || InvalidValue(item.Value))
            {
                continue;
            }
            var expression = BuildExpression<T>(pExp, item);
            if (item.Children.Count > 0)
            {
                var childExpression = SolveConditionUnit<T>(pExp, item.Children);
                if (childExpression != null)
                    expression = Connect(item.LinkChildren, expression, childExpression);
            }
            returnExpression = Connect(item.LinkType, returnExpression, expression);
        }
        return returnExpression;
    }

    private static readonly MethodInfo ContainMethod = typeof(string).GetMethod(nameof(string.Contains), [typeof(string)])!;

    private static Expression BuildExpression<T>(ParameterExpression pExp, ConditionUnit info)
    {
        var propExp = Expression.Property(pExp, info.Name);
        ExpressionType? expType = info.CompareType switch
        {
            CompareType.Contains => null,
            _ => (ExpressionType)Enum.Parse(typeof(ExpressionType), Enum.GetName(info.CompareType)!)
        };
        Expression? exp;
        if (expType.HasValue)
        {
            Expression right;
            var property = typeof(T).GetProperty(info.Name)!;
            object? v;
            if (property.PropertyType.IsEnum)
            {
                v = Enum.Parse(property.PropertyType, info.Value!.ToString()!);
                var enumInt = Expression.Constant((int)v, typeof(int));
                right = Expression.Convert(enumInt, property.PropertyType);
            }
            else
            {
                var type = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
                v = Convert.ChangeType(GetRealValue(info, type), type);
                right = Expression.Constant(v, property.PropertyType);
            }
            exp = Expression.MakeBinary(expType.Value, propExp, right);
        }
        else
        {
            exp = Expression.Call(propExp, ContainMethod, Expression.Constant(GetRealValue(info, typeof(string)), typeof(string)));
        }
        return exp;

        static object GetRealValue(ConditionUnit info, Type type)
        {
            if (info.Value is not JsonElement e)
            {
                return info.Value!;
            }
            else
            {
                return GetJsonValue(e, type);
            }
        }

        static object GetJsonValue(JsonElement e, Type type)
        {
            var code = Type.GetTypeCode(type);
            switch (code)
            {
                case TypeCode.Boolean:
                    return e.GetBoolean();
                case TypeCode.SByte:
                    e.TryGetSByte(out var sbyteValue);
                    return sbyteValue;
                case TypeCode.Byte:
                    e.TryGetSByte(out var byteValue);
                    return byteValue;
                case TypeCode.Int16:
                    e.TryGetInt16(out var int16Value);
                    return int16Value;
                case TypeCode.UInt16:
                    e.TryGetUInt16(out var uint16Value);
                    return uint16Value;
                case TypeCode.Int32:
                    e.TryGetInt32(out var int32Value);
                    return int32Value;
                case TypeCode.UInt32:
                    e.TryGetUInt32(out var uint32Value);
                    return uint32Value;
                case TypeCode.Int64:
                    e.TryGetInt64(out var int64Value);
                    return int64Value;
                case TypeCode.UInt64:
                    e.TryGetUInt64(out var uint64Value);
                    return uint64Value;
                case TypeCode.Single:
                    e.TryGetSingle(out var singleValue);
                    return singleValue;
                case TypeCode.Double:
                    e.TryGetDouble(out var doubleValue);
                    return doubleValue;
                case TypeCode.Decimal:
                    e.TryGetDecimal(out var decimalValue);
                    return decimalValue;
                case TypeCode.DateTime:
                    e.TryGetDateTime(out var dateValue);
                    return dateValue;
                default:
                    return e.ToString();
            }

        }
    }

    static bool InvalidValue(object? value)
    {
        return value == null
               || string.IsNullOrEmpty(value?.ToString())
               || (value is JsonElement e && (e.ValueKind == JsonValueKind.Undefined || e.ValueKind == JsonValueKind.Null));
    }

    static readonly Dictionary<LinkType, ExpressionType> LinkTypeMap = new()
    {
        [LinkType.AndAlso] = ExpressionType.AndAlso,
        [LinkType.OrElse] = ExpressionType.OrElse
    };
    private static Expression Connect(LinkType linkType, Expression? expression, Expression nextExp)
    {
        if (expression == null || linkType == LinkType.None)
        {
            return nextExp;
        }
        return Expression.MakeBinary(LinkTypeMap[linkType], expression, nextExp);
    }
}