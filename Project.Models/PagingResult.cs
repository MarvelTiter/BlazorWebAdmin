using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Models
{
    public struct PagingResult<T>
    {
        public int TotalRecord { get; set; }
        public IEnumerable<T> Data { get; set; }
    }
}
