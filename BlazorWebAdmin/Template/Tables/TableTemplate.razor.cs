using BlazorWebAdmin.Template.Tables.Setting;
using Microsoft.AspNetCore.Components;
using Project.Models;
using Project.Models.Request;
using System.Data;

namespace BlazorWebAdmin.Template.Tables
{
    public partial class TableTemplate<TData>
    {
        [Parameter]
        public TableOptions<TData> TableOptions { get; set; }
        [Parameter]
        public RenderFragment QueryArea { get; set; }
        IEnumerable<TData> datas;
        bool loading;
        int pageIndex = 1;
        int pageSize = 10;
        int total = 0;
                
        public async Task HandleChange()
        {
            loading = true;
            var result = await TableOptions.DataLoader(pageIndex, pageSize);
            datas = result.Payload.Data;
            total = result.Payload.TotalRecord;
            loading = false;
        }
    }

    public class TableOptions<TData>
    {
        public List<ColumnDefinition> Columns { get; set; }
        public List<ButtonDefinition<TData>> Buttons { get; set; }
        public bool EnableSelection { get; set; } = true;
        public bool IsDataTableSource => typeof(TData) == typeof(DataRow);
        public Func<int, int, Task<QueryResult<PagingResult<TData>>>> DataLoader { get; set; }
        public TableOptions()
        {
            Columns = new List<ColumnDefinition>();
            Buttons = new List<ButtonDefinition<TData>>();
        }

        public TableOptions<TData> AddColumn(string label, string prop, ColumnDefinition? col = null)
        {
            if (col == null)
            {
                col = new ColumnDefinition(label, prop);
            }
            Columns.Add(col);
            return this;
        }

        public TableOptions<TData> AddButton(ButtonDefinition<TData> btn)
        {
            Buttons.Add(btn);
            return this;
        }
    }
}
