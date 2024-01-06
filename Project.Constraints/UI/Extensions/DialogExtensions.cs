using Microsoft.AspNetCore.Components;
using Project.Constraints.UI.Flyout;

namespace Project.Constraints.UI.Extensions
{
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
}
