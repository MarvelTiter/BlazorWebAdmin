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
                //Console.WriteLine("Building");
                //builder.OpenComponent<FormDialogTemplate<TData>>(0);
                //builder.AddComponentParameter(1, nameof(FormDialogTemplate<TData>.DialogModel), p);
                //builder.AddComponentParameter(2, nameof(FormDialogTemplate<TData>.Columns), columns);
                //builder.AddComponentReferenceCapture(3, obj => options.Feedback = (IFeedback<TData>)obj);
                //builder.CloseComponent();
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
