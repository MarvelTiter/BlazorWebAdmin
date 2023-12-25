using Microsoft.AspNetCore.Components;
using Project.AppCore.UI;
using Project.AppCore.UI.Table;
using Project.Models;
using Project.Models.Request;
using System.Linq.Expressions;
using System.Reflection;

namespace Project.Web.Shared.Basic
{
    public abstract class ModelPage<TModel, TQuery> : ComponentBase where TQuery : IRequest, new()
    {
        [CascadingParameter] public IUIService UI { get; set; }
        protected TableOptions<TModel, TQuery> Options { get; set; } = new();
        protected override void OnInitialized()
        {
            base.OnInitialized();
            Options.OnQueryAsync = OnQueryAsync;
            Options.OnExportAsync = OnExportAsync;
            // 被重写了
            Options.ShowExportButton = !GetType().GetMethod(nameof(OnExportAsync))!.IsVirtual;
        }

        protected virtual Task<IQueryCollectionResult<TModel>> OnExportAsync(TQuery query)
        {
            IQueryCollectionResult<TModel>? result = new QueryCollectionResult<TModel>();
            result.Success = false;
            return Task.FromResult(result);
        }

        protected abstract Task<IQueryCollectionResult<TModel>> OnQueryAsync(TQuery query);

        protected RenderFragment TableFragment => UI.BuildTable(Options);
    }

    public class BaseUIComponent : ComponentBase
    {
        [CascadingParameter] public IUIService UI { get; set; }
    }
}
