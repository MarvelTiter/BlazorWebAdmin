using Microsoft.AspNetCore.Components;
using Project.Constraints.UI.Flyout;

namespace Project.Constraints.UI.Extensions;

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