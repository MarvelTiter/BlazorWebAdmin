using BlazorWeb.Shared.Template.Tables;
using Microsoft.AspNetCore.Components;
using Project.AppCore.PageHelper;
using Project.AppCore.Services;
using Project.Models;
using Project.Models.Entities;
using Project.Models.Request;

namespace BlazorWeb.Shared.SystemPermission
{
    public partial class OperationLog : IPageAction
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

        public async Task OnShowAsync()
        {
            await tableOptions.RefreshData();
        }

        public Task OnHiddenAsync()
        {
            return Task.CompletedTask;
        }
    }
}
