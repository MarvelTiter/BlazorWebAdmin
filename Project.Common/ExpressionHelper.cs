using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Project.Common
{
    public static class ExpressionHelper
    {
        public static Expression<Func<T, bool>> AndCondition<T>(this Expression<Func<T, bool>> self, Expression<Func<T, bool>> other)
        {
            var p = self.Parameters;
            var body = Expression.AndAlso(self.Body, other.Body);
            return Expression.Lambda<Func<T, bool>>(body, p);
        }
        public static PropertyInfo ExtractProperty<T, TValue>(this Expression<Func<T, TValue>> selector)
        {
            ArgumentNullException.ThrowIfNull(selector);

            if (selector.Body is not MemberExpression body || body.Member is not PropertyInfo prop)
                throw new ArgumentException($"The parameter selector '{selector}' does not resolve to a public property on the type '{typeof(T)}'.", nameof(selector));

            var type = typeof(T);
            var propertyInfo = prop.DeclaringType != type
                             ? type.GetProperty(prop.Name, prop.PropertyType)
                             : prop;

            ArgumentNullException.ThrowIfNull(propertyInfo);
            return propertyInfo;
        }
    }
}
