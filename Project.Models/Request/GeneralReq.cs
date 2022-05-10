using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Project.Models.Request
{
    public interface IRequest
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public Expression? Expression { get; set; }
    }
    public interface IRequest<T> : IRequest
    {
        public new Expression<Func<T, bool>>? Expression { get; set; }
    }
    public class GeneralReq<T> : IRequest<T>
    {
        public string Keyword { get; set; }
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public DateTime StartTime { get; set; } = DateTime.Now;
        public DateTime EndTime { get; set; } = DateTime.Now;
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
    public class GeneralReq : GeneralReq<object> { }
}
