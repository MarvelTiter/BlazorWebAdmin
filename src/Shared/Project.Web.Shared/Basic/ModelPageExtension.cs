using Project.Constraints.Models.Request;
using Project.Constraints.UI.Extensions;
using Project.Constraints.UI.Flyout;

namespace Project.Web.Shared.Basic;

public static class ModelPageExtension
{
    public static async Task<TModel> ShowEditFormAsync<TModel, TQuery>(this ModelPage<TModel, TQuery> page, string title, TModel? data, bool? edit = null, string? width = null)
        where TQuery : IRequest, new()
        where TModel : class, new()
    {
        var n = await page.UI.ShowFormDialogAsync(title, data, page.Options.Columns, edit, width);
        return n;
    }

    public static async Task<TModel> ShowEditFormAsync<TModel, TQuery>(this ModelPage<TModel, TQuery> page, TModel? data, bool? edit = null, Action<FlyoutOptions<TModel>>? config = null)
        where TQuery : IRequest, new()
        where TModel : class, new()
    {
        var n = await page.UI.ShowFormDialogAsync(data, page.Options.Columns, true, config);
        return n;
    }

    public static async Task<TModel> ShowAddFormAsync<TModel, TQuery>(this ModelPage<TModel, TQuery> page, string title, string? width = null)
        where TQuery : IRequest, new()
        where TModel : class, new()
    {
        var n = await page.UI.ShowFormDialogAsync<TModel>(title, null, page.Options.Columns, false, width);
        return n;
    }

    public static async Task<TModel> ShowAddFormAsync<TModel, TQuery>(this ModelPage<TModel, TQuery> page, Action<FlyoutOptions<TModel>> config)
        where TQuery : IRequest, new()
        where TModel : class, new()
    {
        var n = await page.UI.ShowFormDialogAsync(null, page.Options.Columns, false, config);
        return n;
    }
}