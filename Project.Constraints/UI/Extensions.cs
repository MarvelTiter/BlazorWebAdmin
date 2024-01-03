using Microsoft.AspNetCore.Components;
using Project.Constraints.UI.Flyout;
using Project.Constraints.UI.Form;
using Project.Constraints.UI.Table;

namespace Project.Constraints.UI
{

    public static class MessageExtensions
    {
        public static void Success(this IUIService service, string message)
        {
            service.Message(MessageType.Success, message);
        }

        public static void Info(this IUIService service, string message)
        {
            service.Message(MessageType.Information, message);
        }

        public static void Error(this IUIService service, string message)
        {
            service.Message(MessageType.Error, message);
        }
    }

    public static class BuildExtension
    {
        public static async Task<TData> ShowFormDialogAsync<TData>(this IUIService service, string title, TData? data, IEnumerable<ColumnInfo> columns, bool? edit = null, int width = 0) where TData : class, new()
        {
            var options = new FlyoutOptions<TData>();
            options.Title = title;
            if (width > 0) options.Width = width;

            var p = new FormParam<TData>(data, edit);
            options.Content = builder =>
            {
                builder.OpenComponent<FormDialogTemplate<TData>>(0);
                builder.AddComponentParameter(1, nameof(FormDialogTemplate<TData>.DialogModel), p);
                builder.AddComponentParameter(2, nameof(FormDialogTemplate<TData>.Columns), columns);
                builder.AddComponentReferenceCapture(3, obj => options.Feedback = (IFeedback<TData>)obj);
                builder.CloseComponent();
            };
            var result = await service.ShowDialogAsync(options);

            return result;
        }
    }

    public static class DialogExtensions
    {
        public static async Task<TData> ShowDialogAsync<Template, TData>(this IUIService service, string title, TData? param = default, bool? edit = null, int width = 0)
            where Template : DialogTemplate<TData>
        {

            var options = new FlyoutOptions<TData>();
            options.Title = title;
            if (width > 0) options.Width = width;

            var p = new FormParam<TData>(param, edit);
            options.Content = builder =>
            {
                builder.OpenComponent<Template>(0);
                builder.AddComponentParameter(1, nameof(DialogTemplate<TData>.DialogModel), p);
                builder.AddComponentReferenceCapture(2, obj => options.Feedback = (IFeedback<TData>)obj);
                builder.CloseComponent();
            };

            var result = await service.ShowDialogAsync(options);

            return result;
        }

        public static async Task<TData> ShowDialogAsync<TData>(this IUIService service, string title, RenderFragment content, TData? param = default, bool? edit = null, int width = 0)
        {
            var options = new FlyoutOptions<TData>();
            options.Title = title;
            if (width > 0) options.Width = width;

            var p = new FormParam<TData>(param, edit);
            options.Content = builder =>
            {
                builder.OpenComponent<DialogTemplate<TData>>(0);
                builder.AddComponentParameter(1, nameof(DialogTemplate<TData>.DialogModel), p);
                builder.AddComponentParameter(2, nameof(DialogTemplate<TData>.ChildContent), (RenderFragment)(builder => builder.AddContent(0, content)));
                builder.AddComponentReferenceCapture(3, obj => options.Feedback = (IFeedback<TData>)obj);
                builder.CloseComponent();
            };

            var result = await service.ShowDialogAsync(options);

            return result;
        }

    }
}
