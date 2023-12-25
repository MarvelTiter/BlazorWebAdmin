using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Project.Web.Shared.Utils
{
    public static class ExpressionHelper
    {
        public static Expression<Func<T, bool>> AndCondition<T>(this Expression<Func<T,bool>> self, Expression<Func<T,bool>> other)
        {
            var p = self.Parameters;
            var body = Expression.AndAlso(self.Body, other.Body);
            return Expression.Lambda<Func<T, bool>>(body, p);
        }
    }
}
