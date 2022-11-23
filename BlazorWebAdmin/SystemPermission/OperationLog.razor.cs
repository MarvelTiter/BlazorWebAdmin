using BlazorWeb.Shared.Template.Tables;
using Microsoft.AspNetCore.Components;
using Project.AppCore.Services;
using Project.Models;
using Project.Models.Entities;
using Project.Models.Request;

namespace BlazorWebAdmin.SystemPermission
{
    public partial class OperationLog
    {
        [Inject]
        public IRunLogService RunLogSrv { get; set; }
        TableOptions<RunLog, GenericRequest<RunLog>> tableOptions = new TableOptions<RunLog, GenericRequest<RunLog>>();
        protected override void OnInitialized()
        {
            base.OnInitialized();
            tableOptions.LoadDataOnLoaded = true;
            tableOptions.DataLoader = Search;
        }

        Task<IQueryCollectionResult<RunLog>> Search(GenericRequest<RunLog> req)
        {
            return RunLogSrv.GetRunLogsAsync(req);
        }
    }
}
