using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Models
{
    public class QueryResult<T>
    {
        public bool Success { get; set; } = false;
        public string Message { get; set; } = "Error";
        public T Payload { get; set; }
    }
}
