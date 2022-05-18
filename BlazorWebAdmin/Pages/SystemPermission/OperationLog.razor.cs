using BlazorWebAdmin.Template.Tables;
using Microsoft.AspNetCore.Components;
using Project.Models;
using Project.Models.Entities;
using Project.Models.Request;
using Project.Services.interfaces;

namespace BlazorWebAdmin.Pages.SystemPermission
{
    public partial class OperationLog
    {
        [Inject]
        public IRunLogService RunLogSrv { get; set; }
        TableOptions<RunLog, GeneralReq<RunLog>> tableOptions = new TableOptions<RunLog, GeneralReq<RunLog>>();
        protected override void OnInitialized()
        {
            base.OnInitialized();
            tableOptions.LoadDataOnLoaded = true;
            tableOptions.DataLoader = Search;
        }

        Task<IQueryCollectionResult<RunLog>> Search(GeneralReq<RunLog> req)
        {
            return RunLogSrv.GetRunLogsAsync(req);
        }
    }
}
