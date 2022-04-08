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

        public static QueryResult<T> SuccessResult(T payload, string msg = "")
        {
            return new QueryResult<T>()
            {
                Success = true,
                Message = msg,
                Payload = payload,
            };
        }
        public static QueryResult<T> FailResult(T payload, string msg = "")
        {
            return new QueryResult<T>()
            {
                Success = false,
                Message = msg,
                Payload = payload,
            };
        }

        public static QueryResult<PagingResult<T>> PagingResult(IEnumerable<T> payload, int total)
        {
            return new QueryResult<PagingResult<T>>
            {
                Success = true,
                Message = "",
                Payload = new PagingResult<T>
                {
                    TotalRecord = total,
                    Data = payload
                }
            };
        }
    }
}
