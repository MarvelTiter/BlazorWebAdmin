using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Models.Request
{
    public interface IRequest
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
    }
    public class GeneralReq : IRequest
    {
        public string Keyword { get; set; }
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public DateTime Start { get; set; } = DateTime.Now;
        public DateTime End { get; set; } = DateTime.Now;
        public int From => (PageIndex - 1) * PageSize;
        public int To => PageIndex * PageSize;
    }
}
