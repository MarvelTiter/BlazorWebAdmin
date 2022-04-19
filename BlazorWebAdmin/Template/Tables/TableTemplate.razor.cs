using AntDesign.TableModels;
using BlazorWebAdmin.Template.Tables.Setting;
using Microsoft.AspNetCore.Components;
using Project.Models;
using Project.Models.Request;
using System.Data;

namespace BlazorWebAdmin.Template.Tables
{
    public partial class TableTemplate<TData, TQuery> where TQuery : IRequest, new()
    {
        [Parameter]
        public TableOptions<TData, TQuery> TableOptions { get; set; }
        [Parameter]
        public RenderFragment<TQuery> QueryArea { get; set; }

        bool loading;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            if (TableOptions.LoadDataOnLoaded)
            {
                await Search();
            }
        }

        public async Task Search()
        {
            loading = true;
            var result = await TableOptions.DataLoader(TableOptions.Query);
            TableOptions.Datas = result.Payload.Data;
            TableOptions.Total = result.Payload.TotalRecord;
            loading = false;
        }

        public async Task HandleChange()
        {
           if (TableOptions.Page)
            {
                await Search();
            }
        }
    }

    public class TableOptions<TData, TQuery> where TQuery : IRequest, new()
    {
        public List<ColumnDefinition> Columns { get; set; }
        public List<ButtonDefinition<TData>> Buttons { get; set; }
        public string ScrollX { get; set; }
        public bool Page { get; set; } = true;
        public TQuery Query { get; set; }
        public bool EnableSelection { get; set; } = true;
        public bool LoadDataOnLoaded { get; set; } = false;
        public int Total { get; set; }
        public IEnumerable<TData> Datas { get; set; }
        public bool IsDataTableSource => typeof(TData) == typeof(DataRow);
        public Func<TQuery, Task<QueryResult<PagingResult<TData>>>> DataLoader { get; set; }
        public bool Initialized => Columns != null && Columns.Count > 0;
        public Func<RowData, Dictionary<string, object>> OnRow { get; set; }
        public TableOptions()
        {
            Buttons = new List<ButtonDefinition<TData>>();
            Query = new TQuery();
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

        public TableOptions<TData, TQuery> AddButton(ButtonDefinition<TData> btn)
        {
            Buttons.Add(btn);
            return this;
        }

        public ColumnDefinition this[string columnName]
        {
            get
            {
                var col = Columns.First(c=>c.PropertyOrFieldName == columnName);
                if (col == null) throw new InvalidOperationException();
                return col;
            }
        }
    }
}
