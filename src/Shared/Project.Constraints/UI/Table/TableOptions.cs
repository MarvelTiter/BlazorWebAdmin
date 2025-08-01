﻿using Project.Constraints.Common;
using Project.Constraints.Models.Request;
using Project.Constraints.Utils;
using System.Data;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Components;

namespace Project.Constraints.UI.Table;

public static class TableOptionsExtensions
{
    public static void AddColumns(this TableOptions option, (string Title, string Property)[] columns, Action<ColumnInfo>? action = null)
    {
        option.Columns =
        [
            ..columns.Select(c =>
            {
                var col = new ColumnInfo(c.Title, c.Property);
                action?.Invoke(col);
                return col;
            })
        ];
    }
}

public class Grouping<TKey, TElement>(TKey key, TElement[]? items)
{
    public TKey Key { get; set; } = key;
    public TElement[]? Items { get; set; } = items;
}

public class TableOptions
{
    public string? ScrollX { get; set; }
    public Func<Task> NotifyChanged { get; set; } = () => Task.CompletedTask;
    public bool Loading { get; set; }
    public bool EnableSelection { get; set; }
    public bool LoadDataOnLoaded { get; set; }
    public bool FirstRender { get; set; } = true;
    public bool EnabledAdvancedQuery { get; set; } = true;
    public bool EnableRowClick { get; set; }
    public bool ShowAddButton { get; set; }
    public bool ShowExportButton { get; set; }
    public bool ShowImportButton { get; set; }

    /// <summary>
    /// 设置按钮列的宽度，默认170
    /// </summary>
    public string ActionColumnWidth { get; set; } = "170";

    /// <summary>
    /// 启用grid布局
    /// </summary>
    public int? ActionButtonColumn { get; set; }

    public bool Exportable { get; set; }
    public bool AutoRefreshData { get; set; } = true;
    [NotNull] public ColumnInfo[]? Columns { get; set; }
}

public class TableOptions<TData, TQuery> : TableOptions where TQuery : IRequest, new()
{
    private bool pager = true;

    public TableOptions()
    {
        Query = new TQuery();
        if (typeof(TData) == typeof(DataRow) || typeof(TData) == typeof(Dictionary<string, object>))
            return;
        Columns = typeof(TData).GenerateColumns();
    }
    public bool Pager
    {
        get => pager;
        set
        {
            pager = value;
            Query.PageSize = value ? 10 : int.MaxValue;
        }
    }
    public TQuery Query { get; set; }
    public QueryCollectionResult<TData>? Result { get; set; }
    public Func<TData, object> RowKey { get; set; } = d => d!;
    public Func<TData, IEnumerable<TData>> TreeChildren { get; set; } = t => [];
    public IEnumerable<TData> Selected { get; set; } = [];
    public Func<Task<IQueryResult?>>? OnAddItemAsync { get; set; }
    public Func<TQuery, Task<QueryCollectionResult<TData>>>? OnQueryAsync { get; set; }
    public Func<TQuery, Task<QueryCollectionResult<TData>>>? OnExportAsync { get; set; }
    public Func<Stream, Task>? OnImportAsync { get; set; }
    public Func<TData, Task>? OnRowClickAsync { get; set; }
    public Func<TData, Dictionary<string, object>?>? AddRowOptions { get; set; }
    public Func<string, IEnumerable<TData>, Task>? ExportIntercept { get; set; }
    public List<TableButton<TData>>? Buttons { get; set; }
    public Func<TableButton<TData>, IQueryResult, Task>? OnTableButtonClickAsync { get; set; }
    public Func<IEnumerable<TData>, Task>? OnSaveExcelAsync { get; set; }
    public Func<IEnumerable<TData>, Task>? OnSelectedChangedAsync { get; set; }

    public RenderFragment<Grouping<object, TData>>? GroupTitleTemplate { get; set; }
    public RenderFragment<Grouping<object, TData>>? GroupFooterTemplate { get; set; }

    //public Func<Task> RefreshAsync { get; set; }
    public ColumnInfo this[string name]
    {
        get { return Columns.FirstOrDefault(c => c.PropertyOrFieldName == name) ?? throw new ArgumentException($"属性[{name}]未配置[ColumnDefinitionAttribute]或[DisplayAttribute]或[FormAttribute], 因此未被收集到TableOptions.Columns中"); }
    }

    public ColumnInfo GetColumn<TMember>(Expression<Func<TData, TMember>> expression)
    {
        var prop = (expression).ExtractProperty();
        return this[prop.Name];
    }

    public async Task RefreshAsync()
    {
        using var _ = BooleanStatusManager.New((b) => Loading = b, callback: NotifyChanged);
        await NotifyChanged.Invoke();
        //await Task.Yield();
        if (OnQueryAsync != null)
        {
            var result = await OnQueryAsync(Query);
            Result = result;
        }
    }

    public async Task ExportAsync()
    {
        using var _ = BooleanStatusManager.New(b => Loading = b, callback: NotifyChanged);
        await NotifyChanged.Invoke();
        QueryCollectionResult<TData> datas;
        if (OnExportAsync != null)
        {
            datas = await OnExportAsync(Query);
        }
        else
        {
            datas = QueryResult.EmptyResult<TData>();
        }

        var exportDatas = datas.IsSuccess ? datas.Payload : Result?.Payload ?? [];

        if (exportDatas.Any() && OnSaveExcelAsync != null)
        {
            await OnSaveExcelAsync.Invoke(exportDatas);
        }
    }
}