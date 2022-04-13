using BlazorWebAdmin.Template.Tables.Setting;
using Microsoft.AspNetCore.Components;
using System.Data;

namespace BlazorWebAdmin.Template.Tables
{
    public partial class TableTemplate<TData>
    {
        [Parameter]
        public TableOptions<TData> TableOptions { get; set; }
        IEnumerable<TData> datas;
        bool loading;
        int pageIndex = 1;
        int pageSize = 10;
        public async Task HandleChange()
        {
            loading = true;
            datas = await TableOptions.DataLoader(pageIndex, pageSize);
            loading = false;
        }
    }

    public class TableOptions<TData>
    {
        public List<ColumnDefinition> Columns { get; set; }
        public bool EnableSelection { get; set; } = true;
        public bool IsDataTableSource => typeof(TData) == typeof(DataRow);
        public Func<int, int, Task<IEnumerable<TData>>> DataLoader { get; set; }
        public TableOptions()
        {
            Columns = new List<ColumnDefinition>();
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
    }
}
