using System.Data;

namespace Project.Constraints.Models;

public interface IQueryResult
{
    bool Success { get; set; }
    int Code { get; set; }
    string? Message { get; set; }
    object? Payload { get; set; }
}
public interface IQueryResult<T> : IQueryResult
{
    new T? Payload { get; set; }
}

public interface IDataTableResult : IQueryResult<DataTable>
{
    int TotalRecord { get; set; }
}

public interface IQueryCollectionResult<T> : IQueryResult
{
    int TotalRecord { get; set; }
    new IEnumerable<T> Payload { get; set; }
}
[IgnoreAutoInject]
public class QueryResult<T> : IQueryResult<T>
{
    public bool Success { get; set; }
    public int Code { get; set; }
    public string? Message { get; set; }
    public T? Payload { get; set; }
    object? IQueryResult.Payload { get => Payload; set => Payload = (T?)value; }
}

[IgnoreAutoInject]
public class DataTableResult : IDataTableResult
{
    public bool Success { get; set; }
    public int Code { get; set; }
    public string? Message { get; set; }
    public int TotalRecord { get; set; }
    public DataTable? Payload { get; set; }
    object? IQueryResult.Payload { get => Payload; set => Payload = value as DataTable; }
}

[IgnoreAutoInject]
public class QueryCollectionResult<T> : IQueryCollectionResult<T>
{
    public bool Success { get; set; }
    public int Code { get; set; }
    public string? Message { get; set; }
    public int TotalRecord { get; set; }
    public IEnumerable<T> Payload { get; set; } = [];
    object? IQueryResult.Payload { get => Payload; set => Payload = value as IEnumerable<T> ?? []; }
}

[IgnoreAutoInject]
public static class QueryResult
{
    public static IQueryResult Success(string msg = "操作成功")
    {
        return Success<bool>(msg);
    }

    public static IQueryResult Fail(string msg = "操作失败")
    {
        return Fail<bool>(msg);
    }

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

    public static IQueryCollectionResult<T> EmptyResult<T>(string? msg = null)
    {
        return new QueryCollectionResult<T>()
        {
            Success = false,
            Message = msg ?? "列表为空",
            TotalRecord = 0,
            Payload = Enumerable.Empty<T>()
        };
    }

    public static IQueryResult<T> SetPayload<T>(this IQueryResult<T> self, T payload)
    {
        self.Payload = payload!;
        return self;
    }

    public static IQueryCollectionResult<T> CollectionResult<T>(this IQueryResult self, IEnumerable<T> payload)
    {
        return self.CollectionResult(payload, payload.Count());
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

public static class BooleanExtensionForQueryResult
{
    public static IQueryResult<bool> Result(this bool value)
    {
        return QueryResult.Return<bool>(value);
    }

}

public static class TypedResultExtensionForQueryResult
{
    public static IQueryResult<T> Result<T>(this T payload, bool? success = null)
    {
        var s = success ?? payload != null;
        return QueryResult.Return<T>(s).SetPayload(payload);
    }
    public static IQueryResult<T> SetMessage<T>(this IQueryResult<T> self, string message)
    {
        self.Message = message;
        return self;
    }

    public static IDataTableResult Result(this DataTable payload, bool? success = null)
    {
        var s = success ?? payload != null;
        return new DataTableResult
        {
            Success = s,
            Payload = payload
        };
    }

    public static IDataTableResult SetMessage<T>(this IDataTableResult self, string message)
    {
        self.Message = message;
        return self;
    }
}

public static class EnumerableExtensionForQueryResult
{
    public static IQueryCollectionResult<T> CollectionResult<T>(this IEnumerable<T> values, int total = 0)
    {
        if (total == 0) total = values.Count();
        return QueryResult.Success<T>().CollectionResult(values, total);
    }
    public static IQueryCollectionResult<T> CollectionResult<T>(this IEnumerable<T> values, long total)
    {
        return CollectionResult(values, (int)total);
    }
    public static IQueryCollectionResult<T> SetMessage<T>(this IQueryCollectionResult<T> self, string message)
    {
        self.Message = message;
        return self;
    }
}
