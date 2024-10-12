using Microsoft.AspNetCore.Components;
using Project.Constraints.Common.Attributes;
using Project.Constraints.Models;
using Project.Constraints.Models.Request;
using Project.Constraints.UI;
using Project.Constraints.UI.Extensions;
using Project.Constraints.UI.Table;
using Project.Web.Shared.Basic;
using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace BlazorAdmin.Client.TestPages
{
#if DEBUG
    [Route("/test2")]
    [PageInfo(Id = "TestPage2", Title = "测试2", GroupId = "test")]
#endif
    public partial class TestPage2
    {
        [Inject, NotNull] IUIService? UI { get; set; }
    }

    public class TestDataTable : DataTableView<GenericRequest>
    {
        protected override void OnInitialized()
        {
            base.OnInitialized();
            Options.LoadDataOnLoaded = true;
        }
        [TableButton(Label = "测试")]
        public Task<IQueryResult?> RowBtnTest(DataRow data)
        {
            UI.Success(data["姓名"]?.ToString() ?? "");
            return QueryResult.Null().AsTask();
        }
        protected override IEnumerable<(string Title, string Property)> SetColumns()
        {
            yield return ("姓名", "姓名");
            yield return ("年龄", "年龄");
            yield return ("生日", "生日");
        }
        protected override async Task<DataTableResult> OnQueryAsync(GenericRequest query)
        {
            await Task.Delay(1);
            DataTable dt = new DataTable();
            dt.Columns.Add("姓名");
            dt.Columns.Add("年龄");
            dt.Columns.Add("生日");

            foreach (var i in Enumerable.Range(1, 20))
            {
                var row = dt.NewRow();
                row["姓名"] = "测试";
                row["年龄"] = 10 + i;
                row["生日"] = "测试";
                dt.Rows.Add(row);
            }
            return dt;
        }
    }
}
