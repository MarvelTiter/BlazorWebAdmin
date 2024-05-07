using Project.Constraints.UI.Flyout;
using Project.Constraints.UI.Form;
using Project.Constraints.UI.Table;

namespace Project.Constraints.UI.Extensions;

public static class BuildFormExtension
{
    public static async Task<TData> ShowFormDialogAsync<TData>(this IUIService service, string title, TData? data, IEnumerable<ColumnInfo> columns, bool? edit = null, string? width = null) where TData : class, new()
    {
        var result = await ShowFormDialogAsync(service, data, columns, edit, config =>
        {
            config.Title = title;
            config.Width = width;
        });
        return result;
    }

    public static async Task<TData> ShowFormDialogAsync<TData>(this IUIService service, TData? data, IEnumerable<ColumnInfo> columns, bool? edit = null, Action<FlyoutOptions<TData>>? config = null)
        where TData : class, new()
    {
        var options = new FlyoutOptions<TData>();
        config?.Invoke(options);
        var p = new FormParam<TData>(data, edit);
        options.Content = builder =>
        {
            builder.Component<FormDialogTemplate<TData>>()
            .SetComponent(c => c.DialogModel, p)
            .SetComponent(c => c.Options, options)
            .SetComponent(c => c.Columns, columns)
            .Build(obj => options.Feedback = (IFeedback<TData>)obj);
        };
        var result = await service.ShowDialogAsync(options);

        return result;
    }
}
