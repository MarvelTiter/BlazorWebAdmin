using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Constraints.Models
{
    public class JsActionResult : IQueryResult
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public object? Payload { get; set; }
        public int Code { get; set; }
    }
    public class JsActionResult<T> : JsActionResult
    {
        public new T? Payload { get; set; }
    }
}
