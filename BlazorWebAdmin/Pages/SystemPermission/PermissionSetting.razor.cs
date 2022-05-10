using BlazorWebAdmin.Template.Tables;
using Project.Models;
using Project.Models.Entities;
using Project.Models.Request;

namespace BlazorWebAdmin.Pages.SystemPermission
{
    public partial class PermissionSetting
    {
        TableOptions<Power, GeneralReq<Power>> tableOptions = new();
        protected override void OnInitialized()
        {
            base.OnInitialized();
            tableOptions.DataLoader = Search;
        }
        Task<QueryResult<PagingResult<Power>>> Search(GeneralReq<Power> req)
        {
            Console.WriteLine(req.Expression);
            return Task.FromResult(QueryResult<Power>.PagingResult(Enumerable.Empty<Power>(), 0));
        }
    }
}
