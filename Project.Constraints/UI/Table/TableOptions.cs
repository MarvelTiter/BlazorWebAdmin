using Microsoft.JSInterop;
using Project.Common;
using Project.Constraints.Models;
using Project.Constraints.Models.Request;
using Project.Constraints.Store;
using System.Linq.Expressions;
using System.Reflection;

namespace Project.Constraints.UI.Table;


public class TableOptions
{
    public bool Pager { get; set; } = true;
    public string ScrollX { get; set; } = "";
    public Action? NotifyChanged { get; set; }
    public bool Loading { get; set; }
    public bool EnableSelection { get; set; }
    public bool LoadDataOnLoaded { get; set; }
    public bool EnabledAdvancedQuery { get; set; } = true;
    public bool EnableRowClick { get; set; }
    public bool ShowAddButton { get; set; }
    public bool ShowExportButton { get; set; }
    public string ActionColumnWidth { get; set; } = "170";
    // TODO 表格按钮列使用grid布局
    public int? ActionButtonColumn { get; set; }
    public bool Exportable { get; set; }
    public bool AutoRefreshData { get; set; } = true;
    public List<ColumnInfo> Columns { get; set; }
}

public class TableOptions<TData, TQuery> : TableOptions where TQuery : IRequest, new()
{
    public TQuery Query { get; set; }
    public IQueryCollectionResult<TData>? Result { get; set; }
    public Func<TData, object> RowKey { get; set; }
    public Func<TData, IEnumerable<TData>> TreeChildren { get; set; } = t => Enumerable.Empty<TData>();
    public IEnumerable<TData> Selected { get; set; } = Enumerable.Empty<TData>();
    public Func<Task<bool>> OnAddItemAsync { get; set; }
    public Func<TQuery, Task<IQueryCollectionResult<TData>>> OnQueryAsync { get; set; }
    public Func<TQuery, Task<IQueryCollectionResult<TData>>> OnExportAsync { get; set; }
    public Func<TData, Task> OnRowClickAsync { get; set; }
    public Func<TData, Dictionary<string, object>?> AddRowOptions { get; set; }
    public Func<string, IEnumerable<TData>, Task> ExportIntercept { get; set; }
    public List<TableButton<TData>>? Buttons { get; set; }
    public Func<TableButton<TData>, bool, Task>? OnTableButtonClickAsync { get; set; }
    public Func<IEnumerable<TData>, Task> OnSaveExcelAsync { get; set; }
    public Func<IEnumerable<TData>, Task> OnSelectedChangedAsync { get; set; }
    public ColumnInfo this[string name]
    {
        get
        {
            return Columns.FirstOrDefault(c => c.PropertyOrFieldName == name) ?? throw new ArgumentException($"属性[{name}]未配置[ColumnDefinitionAttribute]或[DisplayAttribute]或[FormAttribute], 因此未被收集到TableOptions.Columns中");
        }
    }
    public ColumnInfo this[Expression<Func<TData, object>> expression]
    {
        get
        {
            var prop = (expression).ExtractProperty();
            return this[prop.Name];
        }
    }
    public async Task RefreshAsync()
    {
        Loading = true;
        NotifyChanged?.Invoke();
        var result = await OnQueryAsync(Query);
        Result = result;
        Loading = false;
        NotifyChanged?.Invoke();
    }

    public async Task ExportAsync()
    {
        Loading = true;
        NotifyChanged?.Invoke();
        var datas = await OnExportAsync(Query);
        var exportDatas = (datas?.Success ?? false) ? datas.Payload : Result?.Payload ?? Enumerable.Empty<TData>();

        if (exportDatas.Any() && OnSaveExcelAsync != null)
        {
            await OnSaveExcelAsync.Invoke(exportDatas);
        }
        Loading = false;
        NotifyChanged?.Invoke();
    }

    
}
