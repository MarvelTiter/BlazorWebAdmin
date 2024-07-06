using MT.Toolkit.DateTimeExtension;
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

    public class GenericRequest<T> : IRequest<T>
    {
        public string? Keyword { get; set; }
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public bool FixTime { get; set; } = true;

        private DateTime _start = DateTime.Now;
        public DateTime StartTime { get => FixTime ? _start.DayStart() : _start; set => _start = value; }
        private DateTime _end = DateTime.Now;
        public DateTime EndTime { get => FixTime ? _end.DayEnd() : _end; set => _end = value; }
        public Expression<Func<T, bool>>? Expression { get; set; }
        public int From => (PageIndex - 1) * PageSize;
        public int To => PageIndex * PageSize;
        Expression? IRequest.Expression
        {
            get
            {
                return Expression;
            }
            set
            {
                Expression = value as Expression<Func<T, bool>>;
            }
        }
    }
    public class GenericRequest : GenericRequest<object> { }
}
