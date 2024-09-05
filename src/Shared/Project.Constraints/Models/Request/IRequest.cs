using Project.Constraints.UI;
using Project.Constraints.Utils;
using System.Linq.Expressions;

namespace Project.Constraints.Models.Request
{
    public enum SolveType
    {
        All,
        TopOnly,
    }
    public interface IRequest
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        /// <summary>
        /// <para>是否格式化日期 默认 true</para>
        /// <para>StartTime => yyyy-MM-dd 00:00:00</para>
        /// <para>EndTime => yyyy-MM-dd 23:59:59</para>
        /// </summary>
        public bool FixTime { get; set; }
        //public Expression? Expression { get; set; }
        public SolveType ExpressionSolveType { get; set; }
        ConditionUnit Condition { get; set; }
        ConditionUnit? AdditionalCondition { get; set; }
    }
    public interface IRequest<T> : IRequest
    {
    }

    public static class RequestExtensions
    {
        public static Expression<Func<T, bool>> Expression<T>(this IRequest<T> request)
        {
            if (typeof(T) == typeof(object))
            {
                throw new NotSupportedException("");
            }
            if (request.ExpressionSolveType == SolveType.All)
            {
                if (request.AdditionalCondition != null)
                {
                    var u = new ConditionUnit()
                    {
                        Children = [.. request.Condition.Children, request.AdditionalCondition]
                    };
                    return u.BuildExpression<T>();
                }
                return request.Condition.BuildExpression<T>();
            }
            else if (request.ExpressionSolveType == SolveType.TopOnly)
            {
                if (request.AdditionalCondition != null)
                {
                    var u = new ConditionUnit()
                    {
                        Children = [request.Condition, request.AdditionalCondition]
                    };
                    return u.BuildExpression<T>();
                }
                return request.Condition.BuildTopExpression<T>();
            }
            throw new NotSupportedException(nameof(request.ExpressionSolveType));
        }


        public static IRequest<T> AddCondition<T>(this IRequest<T> request, ConditionUnit unit)
        {
            request.AdditionalCondition = unit;
            return request;
        }

        public static IRequest<T> AndAlso<T>(this IRequest<T> request, string name, object value, CompareType compare = CompareType.Equal)
        {
            var unit = new ConditionUnit
            {
                Name = name,
                Value = value,
                CompareType = compare,
                LinkType = LinkType.AndAlso
            };
            request.AdditionalCondition = unit;
            return request;
        }
        public static IRequest<T> OrElse<T>(this IRequest<T> request, string name, object value, CompareType compare = CompareType.Equal)
        {
            var unit = new ConditionUnit
            {
                Name = name,
                Value = value,
                CompareType = compare,
                LinkType = LinkType.OrElse
            };
            request.AdditionalCondition = unit;
            return request;
        }
    }
}
