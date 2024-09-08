using System.Reflection;
using LightExcel;
using Microsoft.AspNetCore.Components.Rendering;
using Project.Constraints.Store.Models;
using Project.Constraints.UI;
using Project.Constraints.UI.Extensions;
using Project.Web.Shared.Utils;

namespace Project.Web.Shared.Basic;

public abstract class ModelPage<TModel, TQuery> : JsComponentBase
    where TQuery : IRequest, new()
{
    [Inject][NotNull] protected IExcelHelper? Excel { get; set; }
    [Inject][NotNull] private IDownloadServiceProvider? DownloadServiceProvider { get; set; }
    [Inject][NotNull] private ILogger<ModelPage<TModel, TQuery>>? Logger { get; set; }
    [CascadingParameter] private IDomEventHandler? DomEvent { get; set; }
    [CascadingParameter] private TagRoute? RouteInfo { get; set; }
    public TableOptions<TModel, TQuery> Options { get; set; } = new();
    protected bool HideDefaultTableHeader { get; set; }

    protected RenderFragment TableFragment => builder =>
    {
        if (!HideDefaultTableHeader)
            builder.AddContent(0, b =>
            {
                b.Component<DefaultTableHeader<TModel, TQuery>>()
                    .SetComponent(c => c.Options, Options)
                    .SetComponent(c => c.DownloadImportTemplate, EventCallback.Factory.Create(this, DownloadImportTemplate))
                    .Build();
            });
        builder.AddContent(1, UI.BuildTable(Options));
    };

    private bool IsOverride(string methodName)
    {
        var method = GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic);
        return method?.DeclaringType != typeof(ModelPage<TModel, TQuery>);
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        LoadJs = false;
        Options.AutoRefreshData = true;
        Options.RowKey = SetRowKey;
        Options.Buttons = this.CollectButtons<TModel>();
        Options.OnQueryAsync = OnQueryAsync;
        Options.OnAddItemAsync = OnAddItemAsync;
        Options.OnRowClickAsync = OnRowClickAsync;
        Options.AddRowOptions = OnAddRowOptions;
        Options.OnExportAsync = OnExportAsync;
        Options.OnImportAsync = OnImportAsync;
        Options.OnSaveExcelAsync = OnSaveExcelAsync;
        Options.OnSelectedChangedAsync = OnSelectedChangedAsync;
        // 被重写了
        Options.ShowExportButton = IsOverride(nameof(OnExportAsync));
        Options.ShowAddButton = IsOverride(nameof(OnAddItemAsync));
        Options.ShowImportButton = IsOverride(nameof(HandleImportedDataAsync));
        //DomEvent.OnKeyDown += DomEvent_OnKeyDown;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if (firstRender)
        {
            if (Options.LoadDataOnLoaded)
            {
                await Options.RefreshAsync();
            }
        }
    }

    protected virtual object SetRowKey(TModel model)
    {
        return model!;
    }

    /// <summary>
    ///     设置行属性
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    protected virtual Dictionary<string, object>? OnAddRowOptions(TModel model)
    {
        return null;
    }

    /// <summary>
    ///     行点击处理
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    protected virtual Task OnRowClickAsync(TModel model)
    {
        return Task.CompletedTask;
    }

    /// <summary>
    ///     处理新增
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    protected virtual Task<bool> OnAddItemAsync()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    ///     获取导出数据
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    protected virtual async Task<QueryCollectionResult<TModel>> OnExportAsync(TQuery query)
    {
        if (Options.Result == null) await Options.RefreshAsync();
        return Options.Result ?? Result.EmptyResult<TModel>();
    }

    /// <summary>
    ///     导出Excel文件
    /// </summary>
    /// <param name="datas"></param>
    /// <returns></returns>
    protected virtual async Task OnSaveExcelAsync(IEnumerable<TModel> datas)
    {
        var service = DownloadServiceProvider.GetService();
        if (service == null) return;
        var mainName = Router.Current?.RouteTitle ?? typeof(TModel).Name;
        var filename = $"{mainName}_{DateTime.Now:yyyyMMdd-HHmmss}.xlsx";
        await service.DownloadAsync(async () =>
        {
            var path = Path.Combine(AppConst.TempFilePath, filename);
            Excel.WriteExcel(path, datas);
            await service.DownloadFileAsync(filename);
        }, async () =>
        {
            using var ms = new MemoryStream();
            Excel.WriteExcel(ms, datas);
            // ms 在writeexcle后已经关闭了
            using var newms = new MemoryStream(ms.ToArray());
            await service.DownloadStreamAsync(filename, newms);
        });
    }

    /// <summary>
    /// </summary>
    /// <param name="enumerable"></param>
    /// <returns></returns>
    protected virtual Task OnSelectedChangedAsync(IEnumerable<TModel> enumerable) =>
        // TODO table的行选择处理
        Task.CompletedTask;

    protected virtual async Task OnImportAsync(Stream stream)
    {
        var datas = Excel.QueryExcel<TModel>(stream, "Sheet1");
        int total = 0, failed = 0;
        foreach (var item in datas)
        {
            var result = await HandleImportedDataAsync(item);
            total++;
            if (!result.Success)
            {
                failed++;
            }
        }
        UI.AlertInfo("导入完成", $"总数：{total} 导入失败：{failed}");
    }

    protected virtual Task<QueryResult> HandleImportedDataAsync(TModel data)
    {
        throw new NotImplementedException();
    }

    protected async Task DownloadImportTemplate()
    {
        var service = DownloadServiceProvider.GetService();
        if (service == null) return;
        List<TModel> datas = [];
        using var ms = new MemoryStream();
        Excel.WriteExcel(ms, datas);
        // ms 在writeexcle后已经关闭了
        using var newms = new MemoryStream(ms.ToArray());
        await service.DownloadStreamAsync("导入模板.xlsx", newms);
    }

    /// <summary>
    /// 查询数据
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    protected abstract Task<QueryCollectionResult<TModel>> OnQueryAsync(TQuery query);

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.AddContent(0, TableFragment);
    }
}