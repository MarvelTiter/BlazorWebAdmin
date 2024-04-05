﻿using Microsoft.AspNetCore.Components;
using Project.Constraints.UI.Flyout;

namespace Project.Constraints.UI.Extensions
{
    public static class DialogExtensions
    {
        public static async Task<TData> ShowDialogAsync<Template, TData>(this IUIService service, string title, TData? param = default, bool? edit = null, string? width = null)
            where Template : DialogTemplate<TData>
        {

            //var options = new FlyoutOptions<TData>();
            //options.Title = title;
            //options.Width = width;

            //var p = new FormParam<TData>(param, edit);
            //options.Content = builder =>
            //{
            //    builder.Component<Template>()
            //    .SetComponent(c => c.DialogModel, p)
            //    .SetComponent(c => c.Options, options)
            //    .Build(obj => options.Feedback = (IFeedback<TData>)obj);
            //};

            //var result = await service.ShowDialogAsync(options);

            //return result;
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
            options.Content = builder =>
            {
                builder.Component<Template>()
                .SetComponent(c => c.DialogModel, p)
                .SetComponent(c => c.Options, options)
                .Build(obj => options.Feedback = (IFeedback<TData>)obj);
            };

            config?.Invoke(options);

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
                builder.Component<DialogTemplate<TData>>()
                .SetComponent(c => c.DialogModel, p)
                .SetComponent(c => c.Options, options)
                .SetComponent(c => c.ChildContent, content)
                .Build(obj => options.Feedback = (IFeedback<TData>)obj);
            };

            var result = await service.ShowDialogAsync(options);

            return result;
        }
    }
}
