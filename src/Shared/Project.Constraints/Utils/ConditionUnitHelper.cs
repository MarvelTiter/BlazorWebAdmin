using Project.Constraints.Models.Request;
using Project.Constraints.UI;
using System.Linq.Expressions;
using System.Reflection;

namespace Project.Constraints.Utils
{
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

        private static Expression? SolveConditionUnit<T>(ParameterExpression pExp, IEnumerable<ConditionUnit> conditions)
        {
            Expression? returnExpression = null;
            foreach (var item in conditions)
            {
                if (string.IsNullOrEmpty(item.Name) || item.Value == null)
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

        public static Expression BuildExpression<T>(ParameterExpression pExp, ConditionUnit info)
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
                    v = Enum.Parse(property.PropertyType, info.Value?.ToString()!);
                    var enumInt = Expression.Constant((int)v, typeof(int));
                    right = Expression.Convert(enumInt, property.PropertyType);
                }
                else
                {
                    var type = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
                    v = Convert.ChangeType(info.Value, type);
                    right = Expression.Constant(v, property.PropertyType);
                }
                exp = Expression.MakeBinary(expType.Value, propExp, right);
            }
            else
            {
                exp = Expression.Call(propExp, ContainMethod, Expression.Constant(info.Value, typeof(string)));
            }
            return exp;
        }

        static Dictionary<LinkType, ExpressionType> LinkTypeMap = new()
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
}
