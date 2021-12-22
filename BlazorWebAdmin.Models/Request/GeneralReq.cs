using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorWebAdmin.Models.Request
{
    public class GeneralReq
    {
        public string Keyword { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int From => (PageIndex - 1) * PageSize;
        public int To => PageIndex * PageSize;
    }
}
