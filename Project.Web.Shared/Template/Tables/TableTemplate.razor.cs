using AntDesign;
using AntDesign.Core.Helpers.MemberPath;
using AntDesign.TableModels;
using BlazorWeb.Shared.Components;
using BlazorWeb.Shared.Template.Tables.Setting;
using BlazorWeb.Shared.Utils;
using LightExcel;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.JSInterop;
using MT.Toolkit.StringExtension;
using Project.Models;
using Project.Models.Request;
using System.Data;
using System.Linq.Expressions;
using BlazorWeb.Shared.Interfaces;
using Project.AppCore.Routers;

namespace BlazorWeb.Shared.Template.Tables
{
    public partial class TableTemplate<TData, TQuery> : IDisposable where TQuery : IRequest, new()
    {
        [Parameter] public TableOptions<TData, TQuery> TableOptions { get; set; }
        [Parameter] public RenderFragment<TQuery> QueryArea { get; set; }
        [Parameter] public RenderFragment<TQuery> TableHeader { get; set; }
        [Inject] public RouterStore RouterStore { get; set; }
        [Inject] public MessageService MessageSrv { get; set; }
        [Inject] public IJSRuntime JSRuntime { get; set; }
        [Inject] public IExcelHelper Excel { get; set; }
        [Inject] ConfirmService ConfirmSrv { get; set; }
        [Inject] ProtectedLocalStorage LocalStorage { get; set; }
        public bool EnableGenerateQuery => (QueryArea == null || TableOptions.EnabledAdvancedQuery) && !TableOptions.IsDataTableSource;
        [CascadingParameter] IDomEventHandler Root { get; set; }
        [CascadingParameter] TagRoute? TagRoute { get; set; }
        [CascadingParameter] IExceptionHandler ExceptionHandler { get; set; }
        bool loading;
        private ConditionInfo? conditionInfo;
        private Expression<Func<TData, bool>>? ConditionExpression = e => true;

        protected override void OnInitialized()
        {
            base.OnInitialized();
            TableOptions.RefreshData = RefreshData;
            Root.OnKeyDown += Root_OnKeyDown;
        }

        private Task Root_OnKeyDown(Microsoft.AspNetCore.Components.Web.KeyboardEventArgs arg)
        {
#if DEBUG
            Console.WriteLine($"=== {typeof(TData).Name},{typeof(TQuery).Name}, Active Status: {TagRoute?.IsActive} ===");
#endif
            if (TagRoute?.IsActive ?? false)
            {
                if (arg.Key == "Enter")
                {
                    return NewSearch();
                }
            }
            return Task.CompletedTask;
        }

        ConditionBuilder insRef;
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
            if (firstRender)
            {
                if (TableOptions.LoadDataOnLoaded)
                {
                    await NewSearch();
                }
                var result = await LocalStorage.GetAsync<ConditionInfo>(cache_key);
                if (result.Success)
                {
                    insRef?.InitStatus(result.Value!);
                }
                StateHasChanged();
            }
        }

        private bool AdvanceModalVisible = false;
        private bool disposedValue;

        public async Task NewSearch()
        {
            TableOptions.Query.PageIndex = 1;
            await Search();
        }

        public async Task Search()
        {
            if (conditionInfo != null)
                TableOptions.Query.Expression = BuildCondition.CombineExpression<TData>(conditionInfo);
            await DoQuery();
        }

        public async Task AdvanceSearch()
        {
            TableOptions.Query.PageIndex = 1;
            TableOptions.Query.Expression = ConditionExpression;
            AdvanceModalVisible = false;
            await DoQuery();
        }
        private async Task DoQuery()
        {
            try
            {
                loading = true;
                var result = await TableOptions.DataLoader(TableOptions.Query);
                TableOptions.Datas = (result.Payload);
                TableOptions.Total = result.TotalRecord;
            }
            catch (Exception ex)
            {
                await ExceptionHandler.HandleExceptionAsync(ex);
            }
            finally { loading = false; }

        }

        public async void AdvanceExport()
        {
            TableOptions.Query.Expression = ConditionExpression;
            AdvanceModalVisible = false;
            await DoExport();
        }

        public async Task Export()
        {
            if (conditionInfo != null)
                TableOptions.Query.Expression = BuildCondition.CombineExpression<TData>(conditionInfo);
            await DoExport();
        }

        private async Task DoExport()
        {
            loading = true;
            var data = TableOptions.Datas;
            if (TableOptions.ExportDataLoader != null)
            {
                //TableOptions.Query.Expression = ConditionExpression;
                var result = await TableOptions.ExportDataLoader(TableOptions.Query);
                data = result.Payload;
            }
            if (data.Any())
            {
                var filename = $"{RouterStore.Current?.RouteTitle ?? "Temp"}_{DateTime.Now:yyyyMMdd-HHmmss}";
                var path = Path.Combine(AppConst.TempFilePath, $"{filename}.xlsx");
                if (TableOptions.ExportHandler != null)
                {
                    await TableOptions.ExportHandler.Invoke(path, data);
                }
                else
                {
                    if (TableOptions.ExcelTemplatePath.IsEnable())
                    {
                        Excel.WriteExcelByTemplate(path, TableOptions.ExcelTemplatePath, data);
                    }
                    else
                    {
                        Excel.WriteExcel(path, data);
                    }
                }
                _ = JSRuntime.DownloadFile(filename, "xlsx");
            }
            else
            {
                _ = MessageSrv.Error("导出数据为空！");
            }
            loading = false;
        }

        public Task OnRowClickHandle(RowData<TData> row)
        {
            if (TableOptions.OnRowClick != null)
            {
                return TableOptions.OnRowClick(row);
            }
            return Task.CompletedTask;
        }

        public async Task RefreshData()
        {
            await Search();
            StateHasChanged();
        }

        readonly string cache_key = $"TableTemplate_{typeof(TData).Name}_ConditionAction";
        async Task CacheInfo(ConditionInfo info)
        {
            await LocalStorage.SetAsync(cache_key, info);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Root.OnKeyDown -= Root_OnKeyDown;
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
    public class TableOptions<TData, TQuery> where TQuery : IRequest, new()
    {

        public List<TableOptionColumn> Columns { get; set; }
        public List<ButtonDefinition<TData>> Buttons { get; set; }
        public Func<TData, IEnumerable<TData>> TreeChildren { get; set; } = t => Enumerable.Empty<TData>();

        public string ScrollX { get; set; }
        public bool Page { get; set; } = true;
        public TQuery Query { get; set; }
        public bool EnableSelection { get; set; } = false;
        public bool LoadDataOnLoaded { get; set; } = false;
        public bool EnabledAdvancedQuery { get; set; } = false;
        public bool DefaultExpandAllRows { get; set; }
        public int Total { get; set; }
        public string ActionColumnWidth { get; set; } = "100";
        public bool Exportable { get; set; }
        public IEnumerable<TData> Selected { get; set; } = Enumerable.Empty<TData>();
        public bool IsDataTableSource => typeof(TData) == typeof(DataRow) || typeof(TData) == typeof(IDictionary<string, object>);
        public bool AutoRefreshData { get; set; } = true;
        public Func<TQuery, Task<IQueryCollectionResult<TData>>> DataLoader { get; set; }
        public Func<TQuery, Task<IQueryCollectionResult<TData>>> ExportDataLoader { get; set; }
        public Func<string, IEnumerable<TData>, Task> ExportHandler { get; set; }
        public string ExcelTemplatePath { get; set; }
        public Func<Task<bool>> AddHandle { get; set; }
        public Func<RowData<TData>, Task> OnRowClick { get; set; }
        public Func<Task> RefreshData { get; set; }
        public bool Initialized => Columns != null && Columns.Count > 0;
        public Func<RowData<TData>, Dictionary<string, object>> OnRow { get; set; }
        public TableOptions() : this(new TQuery()) { }

        public TableOptions(Func<TQuery> creator) : this(creator()) { }

        public TableOptions(TQuery query)
        {
            Query = query;
            Buttons = new List<ButtonDefinition<TData>>();
            if (!IsDataTableSource)
            {
                Columns = typeof(TData).GenerateColumns();
            }
        }

        private List<TData> _datas = new List<TData>();
        public IEnumerable<TData> Datas
        {
            get => _datas;
            set
            {
                _datas.Clear();
                _datas.AddRange(value);
            }
        }


        public TableOptions<TData, TQuery> AddColumn(string label, string prop, TableOptionColumn? col = null)
        {
            if (Columns == null)
            {
                Columns = new List<TableOptionColumn>();
            }
            if (col == null)
            {
                col = new TableOptionColumn(label, prop);
            }
            Columns.Add(col);
            return this;
        }
        public TableOptions<TData, TQuery> AddColumn(TableOptionColumn col)
        {
            if (Columns == null)
            {
                Columns = new List<TableOptionColumn>();
            }
            Columns.Add(col);
            return this;
        }
        public TableOptions<TData, TQuery> AddButton(string label, Func<TData, Task<bool>> handle, string icon = "")
        {
            var btn = new ButtonDefinition<TData>();
            btn.Label = label;
            btn.Icon = icon;
            btn.Callback = handle;
            return AddButton(btn);
        }
        public TableOptions<TData, TQuery> AddButton(Func<TData, string> label, Func<TData, Task<bool>> handle, string icon = "")
        {
            var btn = new ButtonDefinition<TData>();
            btn.LabelFunc = label;
            btn.Icon = icon;
            btn.Callback = handle;
            return AddButton(btn);
        }
        public TableOptions<TData, TQuery> AddButton(ButtonDefinition<TData> btn)
        {
            Buttons.Add(btn);
            return this;
        }


        public TableOptionColumn this[string columnName]
        {
            get
            {
                var col = Columns.First(c => c.PropertyOrFieldName == columnName);
                if (col == null) throw new InvalidOperationException();
                return col;
            }
        }
    }

    public class ExportOption<TQuery, TData>
    {
        public string ExcelTemplatePath { get; set; }
        public Func<TQuery, Task<IQueryCollectionResult<TData>>> ExportDataLoader { get; set; }
    }
}
