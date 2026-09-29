using BlazorTemplate.ClientCore.UI.Flyout;

namespace BlazorTemplate.ClientCore.UI.Extensions;

public static class DialogExtensions
{
    public static Task<TData> ShowDialogAsync<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Template, TData>(this IUIService service, string title, TData? param = default, bool? edit = null, string? width = null)
        where Template : DialogContentBase<TData>
    {
        return service.ShowDialogAsync<Template, TData>(param, edit, config =>
        {
            config.Title = title;
            config.Width = width;
        });
    }

    public static async Task<TData> ShowDialogAsync<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Template, TData>(this IUIService service, TData? param = default, bool? edit = null, Action<FlyoutOptions<Template, TData, TData>>? config = null)
        where Template : DialogContentBase<TData>
    {
        var options = new FlyoutOptions<Template, TData, TData>();
        var p = new FormParam<TData>(param, edit);
        config?.Invoke(options);
        options.Content = builder =>
        {
            var cb = builder.Component<Template>()
                .SetComponent(c => c.DialogModel, p)
                .SetComponent(c => c.Options, options);
            options.ComponentSet?.Invoke(cb);
            cb.Build(obj => options.Feedback = (IFeedback<TData>)obj);
        };

        var result = await service.ShowDialogAsync(options);

        return result;
    }

    public static async Task<TReturn> ShowDialogAsync<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Template, TInput, TReturn>(this IUIService service, TInput data, Action<FlyoutOptions<Template, TInput, TReturn>>? config = null)
        where Template : DialogContentBase<TInput, TReturn>
    {
        var options = new FlyoutOptions<Template, TInput, TReturn>();
        var p = new FormParam<TInput>(data, true);
        config?.Invoke(options);
        options.Content = builder =>
        {
            var cb = builder.Component<Template>()
                .SetComponent(c => c.DialogModel, p)
                .SetComponent(c => c.Options, options);
            options.ComponentSet?.Invoke(cb);
            cb.Build(obj => options.Feedback = (IFeedback<TReturn>)obj);
        };
        var result = await service.ShowDialogAsync(options);
        return result;
    }

    public static async Task<TData> ShowDialogAsync<TData>(this IUIService service, string title, RenderFragment<TData?> content, TData? param = default, bool? edit = null, string? width = null)
    {
        return await ShowDialogAsync(service, content, param, edit, config =>
        {
            config.Title = title;
            config.Width = width;
        });
    }

    public static async Task<TData> ShowDialogAsync<TData>(this IUIService service, RenderFragment<TData?> content, TData? param = default, bool? edit = null, Action<FlyoutOptions<DialogContentBase<TData>, TData, TData>>? config = null)
    {
        var options = new FlyoutOptions<DialogContentBase<TData>, TData, TData>();
        config?.Invoke(options);

        var p = new FormParam<TData>(param, edit);
        options.Content = builder =>
        {
            builder.Component<DialogContentBase<TData>>()
                .SetComponent(c => c.DialogModel, p)
                .SetComponent(c => c.Options, options)
                .SetComponent(c => c.ChildContent, content)
                .Build(obj => options.Feedback = (IFeedback<TData>)obj);
        };

        var result = await service.ShowDialogAsync(options);

        return result;
    }

    public static async Task<TReturn> ShowDialogAsync<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]TInput, TReturn>(this IUIService service, RenderFragment<TInput?> content, TInput data, Action<FlyoutOptions<DialogContentBase<TInput>, TInput, TReturn>>? config = null)
    {
        var options = new FlyoutOptions<DialogContentBase<TInput>, TInput, TReturn>();
        config?.Invoke(options);
        var p = new FormParam<TInput>(data, true);
        options.Content = builder =>
        {
            builder.Component<DialogContentBase<TInput, TReturn>>()
                .SetComponent(c => c.DialogModel, p)
                .SetComponent(c => c.Options, options)
                .SetComponent(c => c.ChildContent, content)
                .Build(obj => options.Feedback = (IFeedback<TReturn>)obj);
        };
        var result = await service.ShowDialogAsync(options);
        return result;
    }
}