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
        public static async Task<TData> ShowFormDialogAsync<TData>(this IUIService service, string title, TData? data, IEnumerable<ColumnInfo> columns, bool? edit = null, string? width = null) where TData : class, new()
        {
            var options = new FlyoutOptions<TData>();
            options.Title = title;
            options.Width = width;

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
        public static async Task<TData> ShowDialogAsync<Template, TData>(this IUIService service, string title, TData? param = default, bool? edit = null, string? width = null)
            where Template : DialogTemplate<TData>
        {

            var options = new FlyoutOptions<TData>();
            options.Title = title;
            options.Width = width;

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

        public static async Task<TData> ShowDialogAsync<TData>(this IUIService service, string title, RenderFragment content, TData? param = default, bool? edit = null, string? width = null)
        {
            var options = new FlyoutOptions<TData>();
            options.Title = title;
            options.Width = width;

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

    public static class DrawerExtensions
    {
        public static async Task ShowDrawerAsync<Template>(this IUIService service, string title, int width = 0, Position position = Position.Right)
        {
            var options = new FlyoutDrawerOptions<int>();
            options.Title = title;
            if (width > 0)
                options.Width = width.ToString();
            options.Position = position;
            options.Content = builder =>
            {
                //builder.OpenComponent<DrawerTemplate<TData>>(0);
                //builder.AddComponentParameter(1, nameof(DrawerTemplate<TData>.DialogModel), p);
                //builder.AddComponentParameter(2, nameof(DrawerTemplate<TData>.OnOk), EventCallback.Factory.Create(options, options.OnOk));
                //builder.AddComponentParameter(3, nameof(DrawerTemplate<TData>.OnCancel), EventCallback.Factory.Create(options, options.OnClose));
                //builder.AddComponentParameter(4, nameof(DrawerTemplate<TData>.ChildContent), (RenderFragment)(builder =>
                //{
                //    builder.OpenComponent(0, typeof(Template));
                //    builder.CloseComponent();
                //}));
                //builder.AddComponentReferenceCapture(5, obj => options.Feedback = (IFeedback<TData>)obj);
                //builder.CloseComponent();
                builder.OpenComponent(0, typeof(Template));
                builder.CloseComponent();
            };

            _ = await service.ShowDrawerAsync(options);
        }

        public static async Task ShowDrawerAsync<Template>(this IUIService service, string title, RenderFragment content, int width = 0, Position position = Position.Right)
        {
            var options = new FlyoutDrawerOptions<int>();
            options.Title = title;
            if (width > 0)
                options.Width = width.ToString();
            options.Position = position;
            options.Content = content;
            _ = await service.ShowDrawerAsync(options);
        }
    }
}
