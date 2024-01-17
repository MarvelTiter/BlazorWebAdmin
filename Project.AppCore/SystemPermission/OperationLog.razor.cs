using Microsoft.AspNetCore.Components;
using Project.Constraints.Models.Permissions;
using Project.Constraints.PageHelper;

namespace Project.AppCore.SystemPermission
{
    public partial class OperationLog : ModelPage<RunLog, GenericRequest<RunLog>>, IPageAction
    {
        [Inject]
        public IRunLogService RunLogSrv { get; set; }
        protected override void OnInitialized()
        {
            base.OnInitialized();
            Options.LoadDataOnLoaded = true;
        }

        public Task OnShowAsync() => Task.CompletedTask;

        public Task OnHiddenAsync() => Task.CompletedTask;

        protected override object SetRowKey(RunLog model) => model.LogId;

        protected override Task<IQueryCollectionResult<RunLog>> OnQueryAsync(GenericRequest<RunLog> query) => RunLogSrv.GetRunLogsAsync(query);

        protected override Task<IQueryCollectionResult<RunLog>> OnExportAsync(GenericRequest<RunLog> query) => base.OnExportAsync(query);

    }
}
