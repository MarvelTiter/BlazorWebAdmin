using LightExcel;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Project.Constraints;
using Project.Constraints.Models;
using Project.Constraints.Models.Request;
using Project.Constraints.UI.Table;
using Project.Web.Shared.ComponentHelper;
using System.Data;

namespace Project.Web.Shared.Basic
{
    public class DataTableView : BasicComponent
    {
        [Parameter] public DataTable? Data { get; set; }
        [Inject] protected IExcelHelper Excel { get; set; }
        [Inject] IDownloadService DownloadService { get; set; }

        public TableOptions<DataRow, GenericRequest> Options { get; set; } = new();

        protected override void OnInitialized()
        {
            base.OnInitialized();
            Options.AutoRefreshData = true;
            Options.RowKey = r => r;
            Options.OnQueryAsync = OnQueryAsync;
            Options.OnExportAsync = OnExportAsync;
            Options.OnSaveExcelAsync = OnSaveExcelAsync;
            Options.ShowExportButton = true;
        }

        protected virtual async Task<IQueryCollectionResult<DataRow>> OnQueryAsync(GenericRequest query)
        {
            await Task.Delay(1);
            if (Data != null)
            {
                return Data.ToEnumerable().CollectionResult();
            }
            return QueryResult.EmptyResult<DataRow>();
        }

        /// <summary>
        /// 获取导出数据
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        protected virtual Task<IQueryCollectionResult<DataRow>> OnExportAsync(GenericRequest query) => OnQueryAsync(query);

        /// <summary>
        /// 导出Excel文件
        /// </summary>
        /// <param name="datas"></param>
        /// <returns></returns>
        protected virtual Task OnSaveExcelAsync(IEnumerable<DataRow> datas)
        {
            var dt = new DataTable();
            dt.Rows.Add(datas);
            var filename = $"{DateTime.Now:yyyyMMdd-HHmmss}.xlsx";
            var path = Path.Combine(AppConst.TempFilePath, filename);
            Excel.WriteExcel(path, dt);
            DownloadService.DownloadAsync(filename);
            return Task.CompletedTask;
        }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.AddContent(1, UI.BuildDynamicTable(Options, Data));
        }
    }
}