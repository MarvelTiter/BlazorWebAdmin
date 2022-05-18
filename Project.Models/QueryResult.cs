using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Models
{
    public interface IQueryResult
    {
        bool Success { get; set; }
        string Message { get; set; }
        object Payload { get; set; }
    }
    public interface IQueryResult<T> : IQueryResult
    {
        new T Payload { get; set; }
    }
    
    public interface IQueryCollectionResult<T> : IQueryResult
    {
        int TotalRecord { get; set; }
        new IEnumerable<T> Payload { get; set; }
    }

    public class QueryResult<T> : IQueryResult<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T Payload { get; set; }
        object IQueryResult.Payload { get => Payload; set => Payload = (T)value; }
    }
       

    public class QueryCollectionResult<T> : IQueryCollectionResult<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public int TotalRecord { get; set; }
        public IEnumerable<T> Payload { get; set; }
        object IQueryResult.Payload { get => Payload; set => Payload = (IEnumerable<T>)value; }
    }

    public static class QueryResult
    {
        public static IQueryResult<T> Success<T>(string msg = "操作成功")
        {
            return new QueryResult<T>()
            {
                Success = true,
                Message = msg,
            };
        }
        public static IQueryResult<T> Fail<T>(string msg = "操作失败")
        {
            return new QueryResult<T>()
            {
                Success = false,
                Message = msg,
            };
        }

        public static IQueryResult<T> Return<T>(bool success)
		{
            if (success)
                return Success<T>();
            else
                return Fail<T>();
		}

        public static IQueryResult<T> SetPayload<T>(this IQueryResult<T> self, T payload)
        {
            self.Payload = payload!;
            return self;
        }

        public static IQueryCollectionResult<T> CollectionResult<T>(this IQueryResult self, IEnumerable<T> payload)
        {
            return new QueryCollectionResult<T>
            {
                Success = self.Success,
                Message = self.Message,
                TotalRecord = payload.Count(),
                Payload = payload
            };
        }

        public static IQueryCollectionResult<T> CollectionResult<T>(this IQueryResult self, IEnumerable<T> payload, int total)
        {
            return new QueryCollectionResult<T>
            {
                Success = self.Success,
                Message = self.Message,
                TotalRecord = total,
                Payload = payload
            };
        }
    }
}
