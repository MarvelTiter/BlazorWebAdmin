﻿using System.Data;

namespace Project.Constraints.Models;

public interface IQueryResult
{
    bool IsSuccess { get; set; }
    int Code { get; set; }
    string? Message { get; set; }
    object? Payload { get; set; }
}

public class QueryResult : IQueryResult
{
    public bool IsSuccess { get; set; }
    public int Code { get; set; }
    public string? Message { get; set; }
    public object? Payload { get; set; }

    public static implicit operator QueryResult(bool value)
    {
        return new QueryResult { Payload = value, IsSuccess = value };
    }

    public static implicit operator QueryResult(int? value)
    {
        return new QueryResult { Payload = value, IsSuccess = value.HasValue };
    }

    public static implicit operator QueryResult(string? value)
    {
        return new QueryResult { Payload = value, IsSuccess = value.ValueEnable() };
    }

    public static QueryResult Success(string msg = "操作成功")
    {
        return Success<object>(msg);
    }

    public static QueryResult Fail(string msg = "操作失败")
    {
        return Fail<object>(msg);
    }

    public static QueryResult<T> Success<T>(string msg = "操作成功")
    {
        return new QueryResult<T>
        {
            IsSuccess = true,
            Message = msg
        };
    }

    public static QueryResult<T> Fail<T>(string msg = "操作失败")
    {
        return new QueryResult<T>
        {
            IsSuccess = false,
            Message = msg
        };
    }

    public static QueryResult Return(bool success)
    {
        if (success)
        {
            return Success();
        }
        else
        {
            return Fail();
        }
    }

    public static QueryResult<T> Return<T>(bool success)
    {
        if (success)
            return Success<T>();
        return Fail<T>();
    }

    public static QueryCollectionResult<T> EmptyResult<T>(string? msg = null)
    {
        return new QueryCollectionResult<T>
        {
            IsSuccess = false,
            Message = msg ?? "列表为空",
            TotalRecord = 0,
            Payload = []
        };
    }
}
public class QueryResult<T> : QueryResult, IQueryResult
{
    public new T? Payload { get; set; }

    object? IQueryResult.Payload
    {
        get => Payload;
        set => Payload = (T?)value;
    }

    public static implicit operator QueryResult<T>(T? value)
    {
        return new QueryResult<T> { Payload = value, IsSuccess = value.ValueEnable() };
    }
}
public class DataTableResult : IQueryResult
{
    public int TotalRecord { get; set; }
    public DataTable? Payload { get; set; }
    public bool IsSuccess { get; set; }
    public int Code { get; set; }
    public string? Message { get; set; }

    object? IQueryResult.Payload
    {
        get => Payload;
        set => Payload = value as DataTable;
    }
}
public class QueryCollectionResult<T> : IQueryResult
{
    public int TotalRecord { get; set; }
    public IEnumerable<T> Payload { get; set; } = [];
    public bool IsSuccess { get; set; }
    public int Code { get; set; }
    public string? Message { get; set; }

    object? IQueryResult.Payload
    {
        get => Payload;
        set => Payload = value as IEnumerable<T> ?? [];
    }

    public static implicit operator QueryCollectionResult<T>(List<T> values)
    {
        return new QueryCollectionResult<T> { Payload = values, IsSuccess = values.Count > 0 };
    }
}
public static class QueryResultExtensions
{
    public static T SetPayload<T>(this T self, object? payload) where T : IQueryResult
    {
        self.Payload = payload!;
        return self;
    }

    public static T SetMessage<T>(this T self, string? message) where T : IQueryResult
    {
        self.Message = message;
        return self;
    }


    public static QueryCollectionResult<T> CollectionResult<T>(this IQueryResult self, IEnumerable<T> payload)
    {
        return self.CollectionResult(payload, payload.Count());
    }

    public static QueryCollectionResult<T> CollectionResult<T>(this IQueryResult self, IEnumerable<T> payload,
        int total)
    {
        return new QueryCollectionResult<T>
        {
            IsSuccess = self.IsSuccess,
            Message = self.Message,
            TotalRecord = total,
            Payload = payload
        };
    }

    public static bool ValueEnable<T>(this T? value)
    {
        if (value is null)
        {
            return false;
        }
        T def = default!;
        return !Equals(value, def);
    }

    public static IQueryResult AndAlso(this IQueryResult self, IQueryResult other)
    {
        return new QueryResult
        {
            IsSuccess = self.IsSuccess && other.IsSuccess,
            Message = $"Result1({self.IsSuccess}): {self.Message}, Result1({other.IsSuccess}): {other.Message}"
        };
    }

    public static IQueryResult OrElse(this IQueryResult self, IQueryResult other)
    {
        return new QueryResult
        {
            IsSuccess = self.IsSuccess || other.IsSuccess,
            Message = $"Result1({self.IsSuccess}): {self.Message}, Result1({other.IsSuccess}): {other.Message}"
        };
    }
}
public static class TypedResultExtensionForQueryResult
{
    public static QueryResult<T> Result<T>(this T payload, bool? success = null)
    {
        var s = success ?? payload != null;
        return QueryResult.Return<T>(s).SetPayload(payload);
    }

    public static DataTableResult TableResult(this DataTable payload, bool? success = null, long? total = 0)
    {
        var s = success ?? payload != null;
        total ??= payload?.Rows.Count ?? 0;
        return new DataTableResult
        {
            IsSuccess = s,
            Payload = payload,
            TotalRecord = (int)total
        };
    }
}
public static class EnumerableExtensionForQueryResult
{
    public static QueryCollectionResult<T> CollectionResult<T>(this IEnumerable<T> values, int total = 0)
    {
        if (total == 0) total = values.Count();
        return QueryResult.Success<T>().CollectionResult(values, total);
    }

    public static QueryCollectionResult<T> CollectionResult<T>(this IEnumerable<T> values, long total)
    {
        return CollectionResult(values, (int)total);
    }

    public static QueryCollectionResult<TTranform> Cast<T, TTranform>(this QueryCollectionResult<T> origin)
    {
        var list = origin.Payload.Cast<TTranform>();
        return list.CollectionResult(origin.TotalRecord).SetMessage(origin.Message);
    }
}