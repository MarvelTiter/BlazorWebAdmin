using System.Linq.Expressions;
using System.Reflection;

namespace BlazorWebAdmin.Template.Tables.Setting
{
    public class BuildCondition
    {
        private static readonly MethodInfo LikeMethod = typeof(DExpSql.SqlFn).GetMethod("Like")!;
        public static Expression BuildExpression<T>(ParameterExpression pExp, ConditionInfo info)
        {
            var propExp = Expression.Property(pExp, info.Name);
            ExpressionType? expType = info.Type switch
            {
                CompareType.Contains => null,
                _ => (ExpressionType)Enum.Parse(typeof(ExpressionType), Enum.GetName<CompareType>(info.Type)!)
            };
            Expression? exp = null;
            if (expType.HasValue)
            {
                exp = Expression.MakeBinary(expType.Value, propExp, Expression.Constant(info.Value, info.ValueType));
            }
            else
            {
                exp = Expression.Call(propExp, LikeMethod, Expression.Constant(info.Value, typeof(string)));
            }
            return exp;
        }

        public static Expression<Func<T, bool>> CombineExpression<T>(Queue<ConditionInfo> infos, Queue<ExpressionType> types)
        {
            var pExp = Expression.Parameter(typeof(T), "p");
            Expression expression = null;
            while (infos.TryDequeue(out var info))
            {
                if (info.Legal)
                {
                    expression = BuildExpression<T>(pExp, info);
                    break;
                }
                else
                {
                    types.Dequeue();
                }
            }
            if (expression == null)
            {
                throw new InvalidDataException();
            }
            while (infos.Count > 0)
            {
                var next = infos.Dequeue();
                var nextExp = BuildExpression<T>(pExp, next);
                var type = types.Dequeue();
                expression = Expression.MakeBinary(type, expression, nextExp);
            }
            return Expression.Lambda<Func<T, bool>>(expression, pExp);
        }

        public static Expression<Func<T, bool>> CombineExpression<T>(ConditionInfo info)
        {
            var infos = new Queue<ConditionInfo>();
            infos.Enqueue(info);
            var types = new Queue<ExpressionType>();
            return CombineExpression<T>(infos, types);
        }
    }
}
