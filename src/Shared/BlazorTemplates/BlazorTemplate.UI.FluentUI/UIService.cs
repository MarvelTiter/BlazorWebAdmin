using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.FluentUI.AspNetCore.Components;
using BlazorTemplate.UI.FluentUI.Components;
using BlazorTemplate.ClientCore.Components;
using System.Linq.Expressions;
using ButtonType = BlazorTemplate.ClientCore.UI.ButtonType;
using IconInfo = BlazorTemplate.ClientCore.UI.IconInfo;
using MessageType = BlazorTemplate.ClientCore.UI.MessageType;
using BlazorTemplate.ClientCore.UI;
using BlazorTemplate.ClientCore.UI.Extensions;
using BlazorTemplate.ClientCore.UI.Flyout;
using BlazorTemplate.ClientCore.UI.Props;
using BlazorTemplate.ClientCore.UI.Builders;
using BlazorTemplate.ClientCore.Models;
using BlazorTemplate.ClientCore.Models.Request;
using BlazorTemplate.ClientCore.UI.Table;
using BlazorTemplate.ClientCore.UI.Dropdown;
using BlazorTemplate.ClientCore.UI.Form;
using BlazorTemplate.ClientCore.Store;
using BlazorTemplate.ClientCore.UI.Tree;
using BlazorTemplate.ClientCore.Services;

namespace BlazorTemplate.UI.FluentUI;

public class UIService(
    IDialogService dialogService,
    INotificationService notificationService,
    IServiceProvider services,
    ISvgIconService svgIconService
) : IUIService
{
    public Action? Update { get; set; }
    public string MainStyle() => string.Empty; //"_content/Microsoft.FluentUI.AspNetCore.Components/css/reboot.css";

    public RenderFragment AddStyles()
    {
        return b =>
        {
            b.Component<VLink>().SetComponent(s => s.Href, "_content/Microsoft.FluentUI.AspNetCore.Components/Microsoft.FluentUI.AspNetCore.Components.bundle.scp.css").Build();
            b.Component<VLink>().SetComponent(s => s.Href, "_content/BlazorTemplate.UI.FluentUI/fluent.css").Build();
        };
    }


    public string DarkStyle() => string.Empty;

    public RenderFragment UIFrameworkJs()
    {
        return b =>
        {
            //b.Component<VScript>().SetComponent(s => s.Src, "_content/Microsoft.FluentUI.AspNetCore.Components/Microsoft.FluentUI.AspNetCore.Components.lib.module.js").Build();
        };
    }

    public RenderFragment BuildIcon(string name)
    {
        throw new NotImplementedException();
    }

    public void Message(MessageType type, string message)
    {
        switch (type)
        {
            case MessageType.Success:
                _ = notificationService.ShowSuccessToastAsync(message, string.Empty);
                break;
            case MessageType.Error:
                _ = notificationService.ShowErrorToastAsync(message, string.Empty);
                break;
            case MessageType.Warning:
                _ = notificationService.ShowWarningToastAsync(message, string.Empty);
                break;
            case MessageType.Information:
                _ = notificationService.ShowInfoToastAsync(message, string.Empty);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }

    public void Notify(MessageType type, string title, string message)
    {
        var t = type switch
        {
            MessageType.Success => ToastIntent.Success,
            MessageType.Error => ToastIntent.Error,
            MessageType.Warning => ToastIntent.Warning,
            MessageType.Information => ToastIntent.Info,
            _ => throw new ArgumentOutOfRangeException(nameof(type))
        };
        _ = notificationService.ShowToastAsync(options =>
        {
            options.Intent = t;
            options.Title = title;
            options.Subtitle = message;
        });
    }

    public void Alert(MessageType type, string title, string message)
    {
        switch (type)
        {
            case MessageType.Success:
                _ = dialogService.ShowSuccessAsync(message, title);
                break;
            case MessageType.Warning:
                _ = dialogService.ShowWarningAsync(message, title);
                break;
            case MessageType.Error:
                _ = dialogService.ShowErrorAsync(message, title);
                break;
            case MessageType.Information:
                _ = dialogService.ShowInfoAsync(message, title);
                break;
            default:
                break;
        }
    }

    public async Task<bool> ConfirmAsync(string title, string message)
    {
        var localizer = ServiceProvider.GetService<IStringLocalizer<object>>()!;
        var r = await dialogService.ShowConfirmationAsync(message, title, localizer["CustomButtons.Ok"], localizer["CustomButtons.Cancel"]);
        return !r.Cancelled;
    }

    public async Task<TReturn> ShowDialogAsync<TContent, TInput, TReturn>(FlyoutOptions<TContent, TInput, TReturn> options) where TContent : IComponent
    {
        var tcs = new TaskCompletionSource<TReturn>();
        var dialogResult = await dialogService.ShowDialogAsync<FluentDialogWrap<TContent, TInput, TReturn>>(d =>
        {
            d.Header.CloseAction.Visible = true;
            d.Header.InfoAction.Visible = true;
            d.Header.Title = options.Title;
            d.Parameters.Add(nameof(FluentDialogWrap<,,>.Options), options);
        });
        if (dialogResult.Cancelled)
        {
            tcs.TrySetCanceled();
        }
        else
        {
            if (dialogResult.Value is not null)
                tcs.TrySetResult((TReturn)dialogResult.Value);
            else
            {
                tcs.TrySetCanceled();
            }
        }

        return await tcs.Task;
    }

    public async Task<TReturn> ShowDrawerAsync<TContent, TInput, TReturn>(FlyoutDrawerOptions<TContent, TInput, TReturn> options) where TContent : IComponent
    {
        // 弹窗重构已回滚：Drawer 仍走 v5 未迁移的旧接口，留待处理。
        throw new NotImplementedException();
    }

    public IServiceProvider ServiceProvider { get; } = services;

    public IBindableInputComponent<DefaultProp, string> BuildInput(object receiver)
    {
        return new BindableComponentBuilder<FluentTextInput, DefaultProp, string>() { Receiver = receiver };
    }

    public IBindableInputComponent<DefaultProp, string> BuildPassword(object receiver)
    {
        return new BindableComponentBuilder<FluentTextInput, DefaultProp, string>(builder => { builder.SetComponent(c => c.TextInputType, TextInputType.Password); })
        { Receiver = receiver };
    }

    public IBindableInputComponent<DefaultProp, TValue> BuildNumberInput<TValue>(object receiver) where TValue : new()
    {
        return new BindableComponentBuilder<FluentNumberInput<TValue>, DefaultProp, TValue>() { Receiver = receiver };
    }

    public IBindableInputComponent<DatePickerProp, DateTime?> BuildDatePicker(object receiver)
    {
        return new BindableComponentBuilder<FluentDatePicker<DateTime?>, DatePickerProp, DateTime?>() { Receiver = receiver };
    }

    public IBindableInputComponent<DatePickerProp, TDate> BuildDatePicker<TDate>(object receiver)
    {
        return new BindableComponentBuilder<FluentDatePicker<DateTime?>, DatePickerProp, TDate>() { Receiver = receiver };
    }

    public IBindableInputComponent<DefaultProp, bool> BuildCheckBox(object receiver)
    {
        return new BindableComponentBuilder<FluentCheckbox, DefaultProp, bool>(builder =>
            {
                if (builder.Model.Label != null)
                {
                    builder.SetComponent(c => c.Label, builder.Model.Label);
                }
            })
        { Receiver = receiver };
    }

    public IBindableInputComponent<SelectProp, TValue> BuildSelect<TValue>(object receiver, SelectItem<TValue>? options)
    {
        if (typeof(TValue).IsEnum && options == null)
        {
            var builder = new BindableComponentBuilder<FluentEnumSelect<TValue>, SelectProp, TValue>()
            { Receiver = receiver };
            builder.Model.BindValueName = "EnumValue";
            builder.Model.StringValue = true;
            return builder;
        }
        else
        {
            return BuildSelect<Options<TValue>, TValue>(receiver, options!);
        }
    }

    public ISelectInput<SelectProp, TItem, TValue> BuildSelect<TItem, TValue>(object receiver,
        IEnumerable<TItem> options)
    {
#pragma warning disable CS8714
        var builder = new SelectComponentBuilder<FluentTypedSelect<TItem, string>, SelectProp, TItem, TValue>
#pragma warning disable CS8714
            (builder =>
            {
                builder.SetComponent(s => s.Items, options);
                if (builder.Model.ValueExpression is LambdaExpression valueLambda)
                {
                    builder.SetComponent(s => s.ItemValue, valueLambda.Compile());
                }

                if (builder.Model.LabelExpression is LambdaExpression labelLambda)
                {
                    builder.SetComponent(s => s.ItemLabel, labelLambda.Compile());
                }
            })
        { Receiver = receiver };
        builder.Model.BindValueName = "SelectedValue";
        builder.Model.StringValue = true;
        return builder;
    }

    public IButtonInput BuildButton(object receiver)
    {
        return new ButtonComponentBuilder<FluentButton>(builder =>
            {
                if (builder.Model.ButtonType == ButtonType.Default)
                {
                    return;
                }

                switch (builder.Model.ButtonType)
                {
                    case ButtonType.Primary:
                        builder.SetComponent(b => b.Appearance, ButtonAppearance.Primary);
                        break;
                    case ButtonType.Danger:
                        builder.SetComponent(b => b.Color, "#ff4d4f");
                        break;
                }

                if (!string.IsNullOrEmpty(builder.Model.Text))
                    builder.TrySet(nameof(FluentButton.ChildContent), builder.Model.Text.AsContent());
            })
        { Receiver = receiver };
    }

    public RenderFragment BuildFakeButton(ButtonProp props)
    {
        return b => b.Span().AddText("NotImplemented").Build();
    }

    public IBindableInputComponent<SwitchProp, bool> BuildSwitch(object receiver)
    {
        return new BindableComponentBuilder<FluentSwitch, SwitchProp, bool>() { Receiver = receiver };
    }

    public RenderFragment BuildTable<TModel, TQuery>(TableOptions<TModel, TQuery> options)
        where TQuery : IRequest, new()
    {
        return builder => builder.Component<FluentTable<TModel, TQuery>>()
            .SetComponent(c => c.Options, options)
            .Build();
    }

    public RenderFragment BuildDynamicTable<TRowData, TQuery>(TableOptions<TRowData, TQuery> options) where TQuery : IRequest, new()
    {
        return b => b.AddContent(1, "NotImplemented");
    }

    public RenderFragment BuildForm<TData>(FormOptions<TData> options) where TData : class, new()
    {
        return b => b.AddContent(1, "NotImplemented");
    }

    public IFormBuilder<TData> BuildForm<TData>(TData data, string? formName = null)
        where TData : class, new()
    {
        return new FluentFormBuilder<TData>(this, data, formName);
    }

    public RenderFragment BuildDropdown(DropdownOptions options)
    {
        return builder => builder.Component<FluentDropdown>()
            .SetComponent(c => c.Options, options)
            .Build();
    }

    public RenderFragment BuildProfile()
    {
        return builder => builder.Component<FluentProfile>().Build();
    }

    public RenderFragment BuildPopover(PopoverOptions options)
    {
        return builder => builder.Component<FluentPopoverHost>()
            .SetComponent(c => c.Options, options)
            .Build();
    }

    public RenderFragment BuildMenu(IRouterStore router, bool horizontal, IAppStore app)
    {
        return builder => builder.Component<FluentUIMenu>()
            .SetComponent(c => c.Router, router)
            .SetComponent(c => c.Horizontal, horizontal)
            .SetComponent(c => c.App, app)
            .Build();
    }

    public RenderFragment BuildLoginForm(Func<LoginFormModel, Task> handleLogin)
    {
        return b => b.Component<FluentLogin>().SetComponent(c => c.HandleLogin, handleLogin).Build();
    }

    public IBindableInputComponent<DefaultProp, string[]> BuildTree<TData>(object revicer, TreeOptions<TData> options)
    {
        throw new NotImplementedException();
    }

    public ISelectInput<SelectProp, TItem, TValue[]> BuildCheckBoxGroup<TItem, TValue>(object receiver,
        IEnumerable<TItem> options)
    {
        throw new NotImplementedException();
    }

    public ISelectInput<SelectProp, TItem, TValue> BuildRadioGroup<TItem, TValue>(object receiver,
        IEnumerable<TItem> options)
    {
        throw new NotImplementedException();
    }

    public IUIComponent<ModalProp> BuildModal()
    {
        return new PropComponentBuilder<FluentDialog, ModalProp>(self => { self.SetComponent(d => d.ChildContent, self.Model.ChildContent); });
    }

    public IUIComponent<GridProp> BuildRow()
    {
        return new PropComponentBuilder<FluentGrid, GridProp>(row => { row.SetComponent(c => c.ChildContent, row.Model.ChildContent); });
    }

    public IUIComponent<GridProp> BuildCol()
    {
        return new PropComponentBuilder<FluentGridItem, GridProp>(col =>
        {
            col.SetComponent(c => c.ChildContent, col.Model.ChildContent);
            if (col.Model.ColSpan > 0)
            {
                col.SetComponent(c => c.Xs, col.Model.ColSpan);
            }
        });
    }

    public IUIComponent<CardProp> BuildCard()
    {
        return new PropComponentBuilder<FluentCard, CardProp>(card => { card.SetComponent(c => c.ChildContent, card.Model.ChildContent); });
    }

    public RenderFragment RenderContainer()
    {
        // v5: 所有 Provider 合并为单个 FluentProviders（dialogs、tooltips、message bars、toasts...）
        return b =>
        {
            b.Component<FluentProviders>().Build();
        };
    }

    public RenderFragment RenderIcon(IconInfo icon)
    {
        var fluentIcon = svgIconService.GetCustomIcon(icon.Name);
        return b => b.Component<FluentIcon<Icon>>()
            .SetComponent(i => i.Value, fluentIcon)
            .Build();
    }

    public int GetMenuWidth(bool collapsed)
    {
        return collapsed ? 42 : 240;
    }

    public IUIComponent<TabsProp> BuildTabs()
    {
        return new PropComponentBuilder<FluentTabs, TabsProp>(self =>
        {
            void tabContent(Microsoft.AspNetCore.Components.Rendering.RenderTreeBuilder b)
            {
                foreach (var item in self.Model.TabContents)
                {
                    b.Component<FluentTab>()
                        .SetComponent(p => p.Header, item.Title)
                        .SetContent(item.Content ?? "".AsContent()).Build();
                }
            }

            self.SetComponent(m => m.ChildContent, tabContent);
        });
    }
}