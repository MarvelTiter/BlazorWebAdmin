using Project.Constraints.UI;
using System.Linq.Expressions;
using System.Reflection;

namespace Project.Web.Shared.Utils
{
    public class BuildCondition
    {
        private static readonly MethodInfo ContainMethod = typeof(string).GetMethod(nameof(string.Contains))!;
        /// <summary>
        /// 根据条件信息构建单个表达式
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pExp"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        public static Expression BuildExpression<T>(ParameterExpression pExp, ConditionInfo info)
        {
            var propExp = Expression.Property(pExp, info.Name);
            ExpressionType? expType = info.Type switch
            {
                CompareType.Contains => null,
                _ => (ExpressionType)Enum.Parse(typeof(ExpressionType), Enum.GetName(info.Type)!)
            };
            Expression? exp = null;
            if (expType.HasValue)
            {
                Expression right;
                object v;
                if (info.ValueType.IsEnum)
                {
                    v = Enum.Parse(info.ValueType, info.Value.ToString()!);
                    var enumInt = Expression.Constant((int)v, typeof(int));
                    right = Expression.Convert(enumInt, info.ValueType);
                }
                else
                {
                    var type = Nullable.GetUnderlyingType(info.ValueType) ?? info.ValueType;
                    v = Convert.ChangeType(info.Value, type);
                    right = Expression.Constant(v, info.ValueType);
                }
                exp = Expression.MakeBinary(expType.Value, propExp, right);
            }
            else
            {
                exp = Expression.Call(propExp, ContainMethod, Expression.Constant(info.Value, typeof(string)));
            }
            return exp;
        }
        /// <summary>
        /// 将所有条件组合成一个表达式
        /// </summary>
        /// <typeparam name="T">参数类型</typeparam>
        /// <param name="infos">所有条件</param>
        /// <param name="types">所有条件关系(And/Or)</param>
        /// <returns></returns>
        /// <exception cref="InvalidDataException"></exception>
        public static Expression<Func<T, bool>> CombineExpression<T>(Queue<ConditionInfo> infos)
        {
            var pExp = Expression.Parameter(typeof(T), "p");
            Expression expression = null;

            while (infos.Count > 0)
            {
                var next = infos.Dequeue();
                if (!next.Legal) continue;
                var nextExp = BuildExpression<T>(pExp, next);
                expression = Connect(next, expression, nextExp);
            }
            if (expression == null)
            {
                return Expression.Lambda<Func<T, bool>>(Expression.MakeBinary(ExpressionType.Equal, Expression.Constant(1), Expression.Constant(1)), pExp);
            }
            return Expression.Lambda<Func<T, bool>>(expression, pExp);
        }

        private static Expression Connect(ConditionInfo next, Expression? expression, Expression nextExp)
        {
            if (expression == null)
            {
                return nextExp;
            }
            return Expression.MakeBinary(next.LinkType!.Value, expression, nextExp);
        }

        public static Expression<Func<T, bool>> CombineExpression<T>(ConditionInfo info)
        {
            var infos = new Queue<ConditionInfo>();
            infos.Enqueue(info);
            var types = new Queue<ExpressionType>();
            return CombineExpression<T>(infos);
        }
    }
}
