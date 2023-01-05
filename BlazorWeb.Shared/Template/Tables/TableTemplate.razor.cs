using AntDesign;
using AntDesign.Core.Helpers.MemberPath;
using AntDesign.TableModels;
using BlazorWeb.Shared.Template.Tables.Setting;
using BlazorWeb.Shared.Utils;
using LightExcel;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Project.AppCore.Store;
using Project.Models;
using Project.Models.Request;
using System.Data;
using System.Linq.Expressions;

namespace BlazorWeb.Shared.Template.Tables
{
    public partial class TableTemplate<TData, TQuery> where TQuery : IRequest, new()
    {
        [Parameter]
        public TableOptions<TData, TQuery> TableOptions { get; set; }
        [Parameter]
        public RenderFragment<TQuery> QueryArea { get; set; }
        [Parameter]
        public RenderFragment Buttons { get; set; }
        [Inject]
        public RouterStore RouterStore { get; set; }
        [Inject]
        public MessageService MessageSrv { get; set; }
        [Inject]
        public IJSRuntime JSRuntime { get; set; }
        [Inject]
        public IExcelHelper Excel { get; set; }
        public bool EnableGenerateQuery => (QueryArea == null || TableOptions.EnabledAdvancedQuery) && !TableOptions.IsDataTableSource;

        bool loading;
        private ConditionInfo conditionInfo;
        private Expression<Func<TData, bool>>? ConditionExpression = e => true;
        Action SetExpression;
        void AssignExpression()
        {
            if (conditionInfo != null)
                ConditionExpression = BuildCondition.CombineExpression<TData>(conditionInfo);
            TableOptions.Query.Expression = ConditionExpression;
        }
        void IgnoreAssign()
        {
            // QueryArea内赋值，不需要内部赋值
        }
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            SetExpression = QueryArea == null ? AssignExpression : IgnoreAssign;
            TableOptions.RefreshData = RefreshData;
            if (TableOptions.LoadDataOnLoaded)
            {
                Console.WriteLine("OnInitializedAsync");
                await Search();
            }
        }
        private bool AdvanceModalVisible = false;
        public async Task Search()
        {
            if (conditionInfo != null)
                TableOptions.Query.Expression = BuildCondition.CombineExpression<TData>(conditionInfo);
            await DoQuery();
        }

        public async Task AdvanceSearch()
        {
            AdvanceModalVisible = false;
            await DoQuery();
        }
        private async Task DoQuery()
        {
            loading = true;
            SetExpression.Invoke();
            var result = await TableOptions.DataLoader(TableOptions.Query);
            TableOptions.Datas = (result.Payload);
            TableOptions.Total = result.TotalRecord;
            loading = false;
        }

        public async void AdvanceExport()
        {
            AdvanceModalVisible = false;
            await Export();
        }

        public async Task Export()
        {
            loading = true;
            if (TableOptions.Page)
            {
                //TableOptions.Query.Expression = ConditionExpression;
                SetExpression.Invoke();
                var result = await TableOptions.ExportDataLoader(TableOptions.Query);
                TableOptions.Datas = result.Payload;
            }
            var data = TableOptions.Datas;
            if (data.Any())
            {
                var folder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tempfile");
                if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
                var path = Path.Combine(folder, $"{RouterStore.Current?.RouteName ?? "Temp"}_{DateTime.Now:yyyyMMdd-HHmmss}.xlsx");
                var excelData = GeneralExcelData(TableOptions.Columns, data);
                Excel.WriteExcel(path, excelData);
                _ = MessageSrv.Success("导出成功！请下载文件！");
                _ = JSRuntime.PushAsync(path);
            }
            else
            {
                _ = MessageSrv.Error("导出数据为空！");
            }
            loading = false;
        }

        public async Task HandleChange(PaginationEventArgs e)
        {
            Console.WriteLine("HandleChange");
            if (TableOptions.Page)
            {
                await Search();
            }
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

        private static IEnumerable<Dictionary<string, object>> GeneralExcelData(List<ColumnDefinition> columns, IEnumerable<TData> data)
        {
            var dataType = typeof(TData);
            var isDataRow = dataType == typeof(DataRow);
            foreach (var item in data)
            {
                var row = new Dictionary<string, object>();
                foreach (var col in columns)
                {
                    var key = isDataRow ? $"['{col.PropertyOrFieldName}']" : col.PropertyOrFieldName;
                    var val = PathHelper.GetDelegate(key, typeof(TData)).Invoke(item);
                    row[col.Label] = val;
                }
                yield return row;
            }
        }
    }


    public class TableOptions<TData, TQuery> where TQuery : IRequest, new()
    {

        public List<ColumnDefinition> Columns { get; set; }
        public List<ButtonDefinition<TData>> Buttons { get; set; }
        //public Expression<Func<TData, bool>> Expression { get; set; }
        public string ScrollX { get; set; }
        public bool Page { get; set; } = true;
        public TQuery Query { get; set; }
        public bool EnableSelection { get; set; } = false;
        public bool LoadDataOnLoaded { get; set; } = false;
        public bool EnabledAdvancedQuery { get; set; } = false;
        public int Total { get; set; }
        public IEnumerable<TData> Selected { get; set; } = Enumerable.Empty<TData>();
        public bool IsDataTableSource => typeof(TData) == typeof(DataRow) || typeof(TData) == typeof(IDictionary<string, object>);
        public bool AutoRefreshData { get; set; } = true;
        public Func<TQuery, Task<IQueryCollectionResult<TData>>> DataLoader { get; set; }
        public Func<TQuery, Task<IQueryCollectionResult<TData>>> ExportDataLoader { get; set; }
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
            get => _datas; set
            {
                _datas.Clear();
                _datas.AddRange(value);
            }
        }


        public TableOptions<TData, TQuery> AddColumn(string label, string prop, ColumnDefinition? col = null)
        {
            if (Columns == null)
            {
                Columns = new List<ColumnDefinition>();
            }
            if (col == null)
            {
                col = new ColumnDefinition(label, prop);
            }
            Columns.Add(col);
            return this;
        }
        public TableOptions<TData, TQuery> AddColumn(ColumnDefinition col)
        {
            if (Columns == null)
            {
                Columns = new List<ColumnDefinition>();
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


        public ColumnDefinition this[string columnName]
        {
            get
            {
                var col = Columns.First(c => c.PropertyOrFieldName == columnName);
                if (col == null) throw new InvalidOperationException();
                return col;
            }
        }
    }
}
