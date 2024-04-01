using Project.Constraints.UI.Flyout;
using Project.Constraints.UI.Form;
using Project.Constraints.UI.Table;

namespace Project.Constraints.UI.Extensions
{
        public static class BuildFormExtension
    {
        public static async Task<TData> ShowFormDialogAsync<TData>(this IUIService service, string title, TData? data, IEnumerable<ColumnInfo> columns, bool? edit = null, string? width = null) where TData : class, new()
        {
            var options = new FlyoutOptions<TData>();
            options.Title = title;
            options.Width = width;

            var p = new FormParam<TData>(data, edit);
            options.Content = builder =>
            {
                builder.Component<FormDialogTemplate<TData>>()
                .SetComponent(c => c.DialogModel, p)
                .SetComponent(c => c.Columns, columns)
                .Build(obj => options.Feedback = (IFeedback<TData>)obj);
            };
            var result = await service.ShowDialogAsync(options);

            return result;
        }
    }
}
