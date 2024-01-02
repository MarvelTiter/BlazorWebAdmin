using Microsoft.JSInterop;
using Project.Constraints.Store;
using Project.Models;
using Project.Models.Request;

namespace Project.Constraints.UI.Table
{
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
        public bool Exportable { get; set; }
        public bool AutoRefreshData { get; set; } = true;
        public List<ColumnInfo> Columns { get; set; }
    }

    public class TableOptions<TData, TQuery> : TableOptions where TQuery : IRequest, new()
    {
        public TQuery Query { get; set; }
        public IQueryCollectionResult<TData>? Result { get; set; }
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
}
