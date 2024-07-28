using LightExcel;
using Microsoft.AspNetCore.Components;
using Project.Constraints;
using Project.Constraints.Models;
using Project.Constraints.Models.Request;
using Project.Constraints.Page;
using Project.Constraints.Store.Models;
using Project.Constraints.UI;
using Project.Constraints.UI.Extensions;
using Project.Constraints.UI.Table;
using System.Reflection;
using Microsoft.AspNetCore.Components.Rendering;

namespace Project.Web.Shared.Basic;

public abstract class ModelPage<TModel, TQuery> : JsComponentBase
    where TQuery : IRequest, new()
{
    [Inject, NotNull] protected IExcelHelper? Excel { get; set; }
    [Inject, NotNull] IDownloadService? DownloadService { get; set; }
    [CascadingParameter] IDomEventHandler? DomEvent { get; set; }
    [CascadingParameter] TagRoute? RouteInfo { get; set; }
    public TableOptions<TModel, TQuery> Options { get; set; } = new();
    protected bool HideDefaultTableHeader { get; set; }
    bool IsOverride(string methodName)
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
        Options.OnSaveExcelAsync = OnSaveExcelAsync;
        Options.OnSelectedChangedAsync = OnSelectedChangedAsync;
        // 被重写了
        Options.ShowExportButton = IsOverride(nameof(OnExportAsync));
        Options.ShowAddButton = IsOverride(nameof(OnAddItemAsync));

        //DomEvent.OnKeyDown += DomEvent_OnKeyDown;
    }

    protected virtual object SetRowKey(TModel model) => model!;

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

    /// <summary>
    /// 设置行属性
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    protected virtual Dictionary<string, object>? OnAddRowOptions(TModel model) => null;

    /// <summary>
    /// 行点击处理
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    protected virtual Task OnRowClickAsync(TModel model) => Task.CompletedTask;

    /// <summary>
    /// 处理新增
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    protected virtual Task<bool> OnAddItemAsync() => throw new NotImplementedException();

    /// <summary>
    /// 获取导出数据
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    protected virtual async Task<QueryCollectionResult<TModel>> OnExportAsync(TQuery query)
    {
        if (Options.Result == null)
        {
            await Options.RefreshAsync();
        }
        return Options.Result ?? Result.EmptyResult<TModel>();
    }

    /// <summary>
    /// 导出Excel文件
    /// </summary>
    /// <param name="datas"></param>
    /// <returns></returns>
    protected virtual Task OnSaveExcelAsync(IEnumerable<TModel> datas)
    {
        var mainName = Router.Current?.RouteTitle ?? typeof(TModel).Name;
        var filename = $"{mainName}_{DateTime.Now:yyyyMMdd-HHmmss}.xlsx";
        var path = Path.Combine(AppConst.TempFilePath, filename);
        Excel.WriteExcel(path, datas);
        DownloadService.DownloadAsync(filename);
        return Task.CompletedTask;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="enumerable"></param>
    /// <returns></returns>
    protected virtual Task OnSelectedChangedAsync(IEnumerable<TModel> enumerable)
    {
        // TODO table的行选择处理
        return Task.CompletedTask;
    }

    /// <summary>
    /// 查询数据
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    protected abstract Task<QueryCollectionResult<TModel>> OnQueryAsync(TQuery query);

    protected RenderFragment TableFragment => builder =>
    {
        if (!HideDefaultTableHeader)
        {
            builder.AddContent(0, b =>
            {
                b.Component<DefaultTableHeader<TModel, TQuery>>()
                    .SetComponent(c => c.Options, Options)
                    .Build();
            });
        }
        builder.AddContent(1, UI.BuildTable(Options));
    };

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.AddContent(0, TableFragment);
    }
}
