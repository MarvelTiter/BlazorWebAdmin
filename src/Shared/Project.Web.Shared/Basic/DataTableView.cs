using LightExcel;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Project.Constraints;
using Project.Constraints.UI.Extensions;
using System.Data;

namespace Project.Web.Shared.Basic
{
    public class DataTableView<TRequest> : BasicComponent where TRequest : IRequest, new()
    {
        [Parameter] public DataTable? Data { get; set; }
        [Parameter] public int Total { get; set; }
        [Parameter] public List<TableButton<DataRow>> Buttons { get; set; } = [];
        [Inject, NotNull] protected IExcelHelper? Excel { get; set; }
        [Inject, NotNull] protected IDownloadService? DownloadService { get; set; }

        public TableOptions<DataRow, TRequest> Options { get; set; } = new();

        protected override void OnInitialized()
        {
            base.OnInitialized();
            Options.AutoRefreshData = true;
            Options.RowKey = r => r;
            Options.Buttons = [.. this.CollectButtons<DataRow>(), .. Buttons];
            Options.OnQueryAsync = InternalQueryAsync;
            Options.OnExportAsync = OnExportAsync;
            Options.OnSaveExcelAsync = OnSaveExcelAsync;
            Options.ShowExportButton = true;
            Options.OnRowClickAsync = OnRowClickAsync;
        }

        protected async Task<QueryCollectionResult<DataRow>> InternalQueryAsync(TRequest query)
        {
            var result = await OnQueryAsync(query);
            Total = 0;
            if (result != null)
            {
                Data = result.Payload;
                Total = result.TotalRecord;
            }
            if (Data != null)
            {
                return Data.ToEnumerable().CollectionResult(Total);
            }
            return QueryResult.EmptyResult<DataRow>();
        }

        protected virtual Task<DataTableResult> OnQueryAsync(TRequest query) => Task.FromResult<DataTableResult>(null!);

        /// <summary>
        /// 获取导出数据
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        protected virtual Task<QueryCollectionResult<DataRow>> OnExportAsync(TRequest query) => InternalQueryAsync(query);

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
            DownloadService.DownloadFileAsync(filename);
            return Task.CompletedTask;
        }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.AddContent(1, TableFragment);
        }

        public virtual Task OnRowClickAsync(DataRow row) => Task.CompletedTask;

        protected RenderFragment TableFragment => UI.BuildDynamicTable(Options, Data);
    }
}