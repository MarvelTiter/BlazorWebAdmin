using Microsoft.AspNetCore.Components;
using Project.Constraints.UI.Flyout;

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
        public static void BuildFormItem(this IUIService service)
        {

        }
    }

    public static class DialogExtensions
    {
        public static async Task<T> ShowDialogAsync<Template, T>(this IUIService service, string title, T? param = default, bool? edit = null, int width = 0)
            where Template : DialogTemplate<T>
        {

            var options = new FlyoutOptions<T>();
            options.Title = title;
            if (width > 0) options.Width = width;

            var p = new FormParam<T>(param, edit);
            options.Content = builder =>
            {
                builder.OpenComponent<Template>(0);
                builder.AddComponentParameter(1, nameof(DialogTemplate<T>.DialogModel), p);
                builder.AddComponentReferenceCapture(2, obj => options.Feedback = (IFeedback<T>)obj);
                builder.CloseComponent();
            };

            var result = await service.ShowDialogAsync<T>(options);

            return result;
        }

        public static async Task<T> ShowDialogAsync<T>(this IUIService service, string title, RenderFragment content, T? param = default, bool? edit = null, int width = 0)
        {
            var options = new FlyoutOptions<T>();
            options.Title = title;
            if (width > 0) options.Width = width;

            var p = new FormParam<T>(param, edit);
            options.Content = builder =>
            {
                builder.OpenComponent<DialogTemplate<T>>(0);
                builder.AddComponentParameter(1, nameof(DialogTemplate<T>.DialogModel), p);
                builder.AddComponentParameter(2, nameof(DialogTemplate<T>.ChildContent), (RenderFragment)(builder => builder.AddContent(0, content)));
                builder.AddComponentReferenceCapture(3, obj => options.Feedback = (IFeedback<T>)obj);
                builder.CloseComponent();
            };

            var result = await service.ShowDialogAsync<T>(options);

            return result;
        }

    }
}
