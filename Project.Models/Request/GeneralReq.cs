using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Project.Models.Request
{
    public interface IRequest<T>
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
		public Expression<Func<T,bool>>? Expression { get; set; }
	}
    public class GeneralReq<T> : IRequest<T>
    {
        public string Keyword { get; set; }
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public DateTime Start { get; set; } = DateTime.Now;
        public DateTime End { get; set; } = DateTime.Now;
		public Expression<Func<T, bool>>? Expression { get ; set; }
        public int From => (PageIndex - 1) * PageSize;
        public int To => PageIndex * PageSize;

	}
}
