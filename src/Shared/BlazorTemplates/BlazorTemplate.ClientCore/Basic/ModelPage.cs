using AutoPageStateContainerGenerator;
using BlazorTemplate.ClientCore.ComponentHelper;
using BlazorTemplate.ClientCore.UI.Extensions;
using BlazorTemplate.ClientCore.Utils;
using LightExcel;
using Microsoft.AspNetCore.Components.Rendering;
using System.Collections.Immutable;
using System.Reflection;

namespace BlazorTemplate.ClientCore.Basic;

public abstract class ModelPage<TModel, TQuery> : JsComponentBase
    where TQuery : IRequest, new()
{
    [Inject][NotNull] protected IExcelHelper? Excel { get; set; }
    [Inject][NotNull] private IDownloadServiceProvider? DownloadServiceProvider { get; set; }
    [Parameter] public RenderFragment? AdditionalHeaderButtons { get; set; }

    [SaveState(InitExpression = "new()")]
    public virtual TableOptions<TModel, TQuery> Options { get; set; } = new();
    protected bool HideDefaultTableHeader { get; set; }

    protected RenderFragment TableFragment => builder =>
    {
        if (!HideDefaultTableHeader)
            builder.AddContent(0, b =>
            {
                b.Component<DefaultTableHeader<TModel, TQuery>>()
                    .SetComponent(c => c.Options, Options)
                    .SetComponent(c => c.DownloadImportTemplate, EventCallback.Factory.Create(this, DownloadImportTemplate))
                    .SetComponent(c => c.ChildContent, AdditionalHeaderButtons)
                    .Build();
            });
        if (HideDefaultTableHeader && AdditionalHeaderButtons is not null)
        {
            builder.Div(AdditionalHeaderButtons).Set("style", "display: flex;justify-content: space-between;align-items: center;width: 100%;margin-bottom: 10px;").Build();
        }
        builder.Component<MCard>().SetContent(UI.BuildTable(Options)).Build();
    };

    protected RenderFragment TableOnly => builder =>
    {
        builder.AddContent(0, UI.BuildTable(Options));
    };

    /// <summary>
    /// 收集本页按钮重新特性
    ///     <para>
    ///         默认实现走反射，作为兜底路径，面向无法被源生成器分析的页面。
    ///     </para>
    ///     <para>
    ///         源生成器会为可分析的派生类生成该方法的重写，在编译期静态构建按钮列表，
    ///         彻底消除运行时反射并让 AOT 裁剪对其无影响。
    ///     </para>
    /// </summary>
    protected virtual void ApplyCapabilities()
    {
        if (IsOverride(nameof(OnSelectedChangedAsync)))
        {
            Options.OnSelectedChangedAsync = OnSelectedChangedAsync;
        }
        if (IsOverride(nameof(OnRowClickAsync)))
        {
            Options.OnRowClickAsync = OnRowClickAsync;
        }
        Options.ShowExportButton = IsOverride(nameof(OnExportAsync));
        Options.ShowAddButton = IsOverride(nameof(OnAddItemAsync));
        Options.ShowImportButton = IsOverride(nameof(HandleImportedDataAsync));
        if (IsOverride(nameof(OnCellUpdateAsync)))
        {
            Options.OnCellUpdateAsync = OnCellUpdateAsync;
        }
        if (IsOverride(nameof(OnRowUpdateAsync)))
        {
            Options.OnRowUpdateAsync = OnRowUpdateAsync;
        }
    }

    private bool IsOverride(string methodName)
    {
        var method = GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic);
        return method?.DeclaringType != typeof(ModelPage<TModel, TQuery>);
    }

    /// <summary>
    ///     收集本页通过 <c>[TableButton]</c> 及其派生特性声明的表格按钮。
    ///     <para>
    ///         默认实现走反射（按特性扫描方法、按名字解析 Label/Visible 表达式方法），
    ///         作为兜底路径，面向无法被源生成器分析的页面。
    ///     </para>
    ///     <para>
    ///         源生成器会为可分析的派生类生成该方法的重写，在编译期静态构建按钮列表，
    ///         彻底消除运行时反射并让 AOT 裁剪对其无影响。
    ///     </para>
    /// </summary>
    protected virtual List<TableButton<TModel>> CollectPageButtons() => this.CollectButtons<TModel>();

    protected override void OnInitialized()
    {
        base.OnInitialized();
        LoadJs = false;
        Options.AutoRefreshData = true;
        Options.RowKey = SetRowKey;
        Options.Buttons = CollectPageButtons();
        Options.OnQueryAsync = OnQueryAsync;
        Options.OnAddItemAsync = OnAddItemAsync;
        Options.AddRowOptions = OnAddRowOptions;
        Options.OnExportAsync = OnExportAsync;
        Options.OnImportAsync = OnImportAsync;
        Options.OnSaveExcelAsync = OnSaveExcelAsync;
        ApplyCapabilities();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if (firstRender)
        {
            if (Options.LoadDataOnLoaded && Options.FirstRender)
            {
                await Options.RefreshAsync();
                Options.FirstRender = false;
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
    protected virtual Task<IQueryResult?> OnCellUpdateAsync(TModel model, ColumnInfo col)
    {
        return QueryResult.Null().AsTask();
    }
    protected virtual Task<IQueryResult?> OnRowUpdateAsync(TModel model, IReadOnlyList<ColumnInfo> cols)
    {
        return QueryResult.Null().AsTask();
    }
    /// <summary>
    /// 处理新增
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    protected virtual Task<IQueryResult?> OnAddItemAsync()
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
        return Options.Result ?? QueryResult.EmptyResult<TModel>();
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
    protected virtual Task<IEnumerable<TModel>> OnSelectedChangedAsync(ImmutableArray<TModel> sources, ImmutableArray<TModel> selected)
    {
        // TODO table的行选择处理
        return Task.FromResult<IEnumerable<TModel>>(selected);
    }

    protected virtual async Task OnImportAsync(Stream stream)
    {
        var datas = Excel.QueryExcel<TModel>(stream, "Sheet1");
        int total = 0, failed = 0;
        foreach (var item in datas)
        {
            var result = await HandleImportedDataAsync(item);
            total++;
            if (!result.IsSuccess)
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