using LightExcel;
using Microsoft.AspNetCore.Components;
using Project.Constraints;
using Project.Constraints.Page;
using Project.Constraints.UI.Table;
using Project.Models;
using Project.Models.Request;
using System.Reflection;

namespace Project.Web.Shared.Basic
{
    public abstract class ModelPage<TModel, TQuery> : BasicComponent where TQuery : IRequest, new()
    {
        [Inject] protected IExcelHelper Excel { get; set; }
        [Inject] IDownloadService DownloadService { get; set; }
        protected TableOptions<TModel, TQuery> Options { get; set; } = new();
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
            Options.NotifyChanged = StateHasChanged;
            Options.Buttons = CollectButtons();
            Options.OnQueryAsync = OnQueryAsync;
            Options.OnAddItemAsync = OnAddItemAsync;
            Options.OnRowClickAsync = OnRowClickAsync;
            Options.AddRowOptions = OnAddRowOptions;
            Options.OnExportAsync = OnExportAsync;
            Options.OnSaveExcelAsync = OnSaveExcelAsync;
            // 被重写了
            Options.ShowExportButton = IsOverride(nameof(OnExportAsync));
            Options.ShowAddButton = IsOverride(nameof(OnAddItemAsync));
        }

        private List<TableButton<TModel>> CollectButtons()
        {
            List<TableButton<TModel>> buttons = new List<TableButton<TModel>>();

            var type = GetType();
            var methods = type.GetMethods().Where(m => m.GetCustomAttribute<TableButtonAttribute>() != null);

            foreach (var method in methods)
            {
                var btnOptions = method.GetCustomAttribute<TableButtonAttribute>()!;
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
                    btn.Visible = ve?.CreateDelegate<Func<TModel, bool>>(this) ?? (t => true);
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
                    await InvokeAsync(StateHasChanged);
                }
            }
        }

        protected virtual Dictionary<string, object>? OnAddRowOptions(TModel model)
        {
            return null;
        }

        protected virtual Task OnRowClickAsync(TModel model)
        {
            return Task.CompletedTask;
        }


        protected virtual Task<bool> OnAddItemAsync()
        {
            throw new NotImplementedException();
        }
        protected virtual Task<IQueryCollectionResult<TModel>> OnExportAsync(TQuery query)
        {
            //IQueryCollectionResult<TModel>? result = new QueryCollectionResult<TModel>();
            //result.Success = false;
            //return Task.FromResult(result);
            throw new NotImplementedException();
        }

        protected virtual Task OnSaveExcelAsync(IEnumerable<TModel> datas)
        {
            var filename = $"{typeof(TModel).Name}_{DateTime.Now:yyyyMMdd-HHmmss}.xlsx";
            var path = Path.Combine(AppConst.TempFilePath, filename);
            Excel.WriteExcel(path, datas);
            DownloadService.DownloadAsync(filename);
            return Task.CompletedTask;
        }

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
}
