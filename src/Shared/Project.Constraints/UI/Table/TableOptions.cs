using Project.Constraints.Common;
using Project.Constraints.Models.Request;
using System.Data;
using System.Linq.Expressions;

namespace Project.Constraints.UI.Table;


public class TableOptions
{
    public bool Pager { get; set; } = true;
    public string ScrollX { get; set; } = "";
    public Func<Task> NotifyChanged { get; set; } = () => Task.CompletedTask;
    public bool Loading { get; set; }
    public bool EnableSelection { get; set; }
    public bool LoadDataOnLoaded { get; set; }
    public bool EnabledAdvancedQuery { get; set; } = true;
    public bool EnableRowClick { get; set; }
    public bool ShowAddButton { get; set; }
    public bool ShowExportButton { get; set; }
    public string ActionColumnWidth { get; set; } = "170";
    /// <summary>
    /// 启用grid布局
    /// </summary>
    public int? ActionButtonColumn { get; set; }
    public bool Exportable { get; set; }
    public bool AutoRefreshData { get; set; } = true;
    public List<ColumnInfo> Columns { get; set; }
}

public class TableOptions<TData, TQuery> : TableOptions where TQuery : IRequest, new()
{
    public TableOptions()
    {
        Query = new TQuery();
        if (typeof(TData) == typeof(DataRow) || typeof(TData) == typeof(Dictionary<string, object>))
            return;
        Columns = typeof(TData).GenerateColumns();
    }
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
    //public Func<Task> RefreshAsync { get; set; }
    public ColumnInfo this[string name]
    {
        get
        {
            return Columns.FirstOrDefault(c => c.PropertyOrFieldName == name) ?? throw new ArgumentException($"属性[{name}]未配置[ColumnDefinitionAttribute]或[DisplayAttribute]或[FormAttribute], 因此未被收集到TableOptions.Columns中");
        }
    }

    public ColumnInfo GetColumn<TMember>(Expression<Func<TData, TMember>> expression)
    {
        var prop = (expression).ExtractProperty();
        return this[prop.Name];
    }

    public async Task RefreshAsync()
    {
        Loading = true;
        await NotifyChanged.Invoke();
        await Task.Yield();
        var result = await OnQueryAsync(Query);
        Result = result;
        Loading = false;
        await NotifyChanged.Invoke();
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
