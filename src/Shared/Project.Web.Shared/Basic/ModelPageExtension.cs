using Project.Constraints.Models.Request;
using Project.Constraints.UI.Extensions;

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

    public static async Task<TModel> ShowAddFormAsync<TModel, TQuery>(this ModelPage<TModel, TQuery> page, string title, string? width = null)
        where TQuery : IRequest, new()
        where TModel : class, new()
    {
        var n = await page.UI.ShowFormDialogAsync<TModel>(title, null, page.Options.Columns, false, width);
        return n;
    }
}