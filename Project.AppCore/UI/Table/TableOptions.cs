using Project.Models;
using Project.Models.Request;

namespace Project.AppCore.UI.Table
{
    public class TableOptions
    {
        public bool Pager { get; set; } = true;
        public string ScrollX { get; set; }
        public bool EnableSelection { get; set; } = false;
        public bool LoadDataOnLoaded { get; set; } = false;
        public bool EnabledAdvancedQuery { get; set; } = false;
        public bool ShowExportButton { get; set; }
        public string ActionColumnWidth { get; set; } = "100";
        public bool Exportable { get; set; }
        public bool AutoRefreshData { get; set; } = true;
    }
    public class TableOptions<TData, TQuery> : TableOptions where TQuery : IRequest, new()
    {
        public TQuery Query { get; set; }
        public IQueryCollectionResult<TData>? Result { get; set; }
        public IEnumerable<TData> Selected { get; set; } = Enumerable.Empty<TData>();
        public Func<TQuery, Task<IQueryCollectionResult<TData>>> OnQueryAsync { get; set; }
        public Func<TQuery, Task<IQueryCollectionResult<TData>>> OnExportAsync { get; set; }
        public Func<string, IEnumerable<TData>, Task> ExportIntercept { get; set; }
        public Func<Task<bool>> OnAddItemAsync { get; set; }



    }
}
