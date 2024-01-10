using LightExcel;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using Project.Constraints;
using Project.Constraints.Models;
using Project.Constraints.Models.Request;
using Project.Constraints.Page;
using Project.Constraints.UI.Extensions;
using Project.Constraints.UI.Table;
using Project.Models.Entities;
using System.Linq.Expressions;
using System.Reflection;

namespace Project.Web.Shared.Basic;

public static class ModelPageExtension
{
    public static async Task<TModel> ShowEditFormAsync<TModel, TQuery>(this ModelPage<TModel, TQuery> page, string title, TModel? data, string? width = null)
         where TQuery : IRequest, new()
        where TModel : class, new()
    {
        var n = await page.UI.ShowFormDialogAsync(title, data, page.Options.Columns, true, width);
        return n;
    }

    public static async Task<TModel> ShowAddFormAsync<TModel, TQuery>(this ModelPage<TModel, TQuery> page, string title,string? width = null)
         where TQuery : IRequest, new()
        where TModel : class, new()
    {
        var n = await page.UI.ShowFormDialogAsync<TModel>(title, null, page.Options.Columns, false, width);
        return n;
    }
}

public abstract class ModelPage<TModel, TQuery> : BasicComponent
    where TQuery : IRequest, new()
{
    [Inject] protected IExcelHelper Excel { get; set; }
    [Inject] IDownloadService DownloadService { get; set; }
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
        Options.Query = new TQuery();
        Options.Columns = typeof(TModel).GenerateColumns();
        Options.AutoRefreshData = true;
        Options.RowKey = SetRowKey;
        Options.Buttons = CollectButtons();
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
    }



    protected abstract object SetRowKey(TModel model);

    private List<TableButton<TModel>> CollectButtons()
    {
        List<TableButton<TModel>> buttons = new List<TableButton<TModel>>();

        var type = GetType();
        var methods = type.GetMethods().Where(m => m.GetCustomAttribute<TableButtonAttribute>() != null);

        foreach (var method in methods)
        {
            var btnOptions = method.GetCustomAttribute<TableButtonAttribute>()!;
            ArgumentNullException.ThrowIfNull(btnOptions.Label ?? btnOptions.LabelExpression);
            var btn = new TableButton<TModel>(btnOptions);
            btn.Callback = method.CreateDelegate<Func<TModel, Task<bool>>>(this);
            if (btnOptions.LabelExpression != null)
            {
                var le = type.GetMethod(btnOptions.LabelExpression);
                btn.LabelExpression = le?.CreateDelegate<Func<TModel, string>>(this);
            }

            if (btnOptions.VisibleExpression != null)
            {
                var ve = type.GetMethod(btnOptions.VisibleExpression);
                btn.Visible = ve?.CreateDelegate<Func<TableButtonContext<TModel>, bool>>(this) ?? (t => true);
            }

            buttons.Add(btn);
        }
        return buttons;
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
    protected virtual Task<IQueryCollectionResult<TModel>> OnExportAsync(TQuery query) => throw new NotImplementedException();

    /// <summary>
    /// 导出Excel文件
    /// </summary>
    /// <param name="datas"></param>
    /// <returns></returns>
    protected virtual Task OnSaveExcelAsync(IEnumerable<TModel> datas)
    {
        var filename = $"{typeof(TModel).Name}_{DateTime.Now:yyyyMMdd-HHmmss}.xlsx";
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
    protected abstract Task<IQueryCollectionResult<TModel>> OnQueryAsync(TQuery query);

    protected RenderFragment TableFragment => builder =>
    {
        if (!HideDefaultTableHeader)
        {
            builder.AddContent(0, UI.BuildTableHeader(Options));
        }
        builder.AddContent(1, UI.BuildTable(Options));
    };
}
