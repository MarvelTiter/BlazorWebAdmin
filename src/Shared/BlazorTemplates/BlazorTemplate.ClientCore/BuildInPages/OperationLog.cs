using AutoPageStateContainerGenerator;

namespace BlazorTemplate.ClientCore.BuildInPages;

public partial class OperationLog<TRunLog, TRunLogService> : ModelPage<TRunLog, GenericRequest<TRunLog>>
    where TRunLog : class, IRunLog, new()
    where TRunLogService : IRunLogService<TRunLog>
{
    [Inject, NotNull] TRunLogService? RunLogSrv { get; set; }
    protected override void OnInitialized()
    {
        base.OnInitialized();
        Options.LoadDataOnLoaded = true;
    }

    protected override object SetRowKey(TRunLog model) => model.LogId;

    protected override Task<QueryCollectionResult<TRunLog>> OnQueryAsync(GenericRequest<TRunLog> query) => RunLogSrv.GetRunLogsAsync(query);

    protected override Task<QueryCollectionResult<TRunLog>> OnExportAsync(GenericRequest<TRunLog> query) => base.OnExportAsync(query);
}

[StateContainer]
public partial class TemplateOperationLog : OperationLog<TemplateRunLog, ITemplateRunLogService>
{
}
