﻿using Microsoft.AspNetCore.Components;
using Project.Constraints.UI.Flyout;

namespace Project.Constraints.UI.Extensions;

public static class DialogExtensions
{
    public static async Task<TData> ShowDialogAsync<Template, TData>(this IUIService service, string title, TData? param = default, bool? edit = null, string? width = null)
        where Template : DialogTemplate<TData>
    {
        return await ShowDialogAsync<Template, TData>(service, param, edit, config =>
        {
            config.Title = title;
            config.Width = width;
        });
    }

    public static async Task<TData> ShowDialogAsync<Template, TData>(this IUIService service, TData? param = default, bool? edit = null, Action<FlyoutOptions<TData>>? config = null)
        where Template : DialogTemplate<TData>
    {
        var options = new FlyoutOptions<TData>();
        var p = new FormParam<TData>(param, edit);
        config?.Invoke(options);
        options.Content = builder =>
        {
            builder.Component<Template>()
                .SetComponent(c => c.DialogModel, p)
                .SetComponent(c => c.Options, options)
                .Build(obj => options.Feedback = (IFeedback<TData>)obj);
        };

        var result = await service.ShowDialogAsync(options);

        return result;
    }

    public static async Task<TReturn> ShowDialogAsync<Template, TInput, TReturn>(this IUIService service, TInput data, Action<FlyoutOptions<TReturn>>? config = null)
        where Template : DialogTemplate<TInput, TReturn>
    {
        var options = new FlyoutOptions<TReturn>();
        var p = new FormParam<TInput>(data, true);
        config?.Invoke(options);
        options.Content = builder =>
        {
            builder.Component<Template>()
                .SetComponent(c => c.DialogModel, p)
                .SetComponent(c => c.Options, options)
                .Build(obj => options.Feedback = (IFeedback<TReturn>)obj);
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

    public static async Task<TData> ShowDialogAsync<TData>(this IUIService service, RenderFragment<TData?> content, TData? param = default, bool? edit = null, Action<FlyoutOptions<TData>>? config = null)
    {
        var options = new FlyoutOptions<TData>();
        config?.Invoke(options);

        var p = new FormParam<TData>(param, edit);
        options.Content = builder =>
        {
            builder.Component<DialogTemplate<TData>>()
                .SetComponent(c => c.DialogModel, p)
                .SetComponent(c => c.Options, options)
                .SetComponent(c => c.ChildContent, content)
                .Build(obj => options.Feedback = (IFeedback<TData>)obj);
        };

        var result = await service.ShowDialogAsync(options);

        return result;
    }

    public static async Task<TReturn> ShowDialogAsync<TInput, TReturn>(this IUIService service, RenderFragment<TInput?> content, TInput data, Action<FlyoutOptions<TReturn>>? config = null)
    {
        var options = new FlyoutOptions<TReturn>();
        config?.Invoke(options);
        var p = new FormParam<TInput>(data, true);
        options.Content = builder =>
        {
            builder.Component<DialogTemplate<TInput, TReturn>>()
                .SetComponent(c => c.DialogModel, p)
                .SetComponent(c => c.Options, options)
                .SetComponent(c => c.ChildContent, content)
                .Build(obj => options.Feedback = (IFeedback<TReturn>)obj);
        };
        var result = await service.ShowDialogAsync(options);
        return result;
    }
}