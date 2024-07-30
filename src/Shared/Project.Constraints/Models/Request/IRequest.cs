using System.Linq.Expressions;

namespace Project.Constraints.Models.Request
{
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
        public Expression? Expression { get; set; }
    }
    public interface IRequest<T> : IRequest
    {
        public new Expression<Func<T, bool>>? Expression { get; set; }
    }

    public static class RequestExtensions
    {
        public static Expression<Func<T, bool>> Expression<T>(this IRequest<T> request)
        {
            return default;
        }
    }
}
