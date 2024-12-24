using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;
using Project.Constraints.Models;
using Project.Constraints.Models.Request;
using Project.Constraints.Store;
using Project.Constraints.UI;
using Project.Constraints.UI.Builders;
using Project.Constraints.UI.Dropdown;
using Project.Constraints.UI.Flyout;
using Project.Constraints.UI.Form;
using Project.Constraints.UI.Props;
using Project.Constraints.UI.Table;
using Project.Constraints.UI.Tree;
using Project.UI.FluentUI.Components;
using System.Linq.Expressions;
using MessageType = Project.Constraints.UI.MessageType;
using ButtonType = Project.Constraints.UI.ButtonType;
using Project.Constraints.UI.Extensions;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.DependencyInjection;
namespace Project.UI.FluentUI;

public class UIService(
    IDialogService dialogService,
    IToastService toastService,
    IServiceProvider services
) : IUIService
{
    public string MainStyle() => string.Empty;//"_content/Microsoft.FluentUI.AspNetCore.Components/css/reboot.css";

    public string DarkStyle() => string.Empty;

    public string UIFrameworkJs() => "_content/Microsoft.FluentUI.AspNetCore.Components/Microsoft.FluentUI.AspNetCore.Components.lib.module.js";
    public void Message(MessageType type, string message)
    {
        switch (type)
        {
            case Constraints.UI.MessageType.Success:
                toastService.ShowSuccess(message);
                break;
            case Constraints.UI.MessageType.Error:
                toastService.ShowError(message);
                break;
            case Constraints.UI.MessageType.Warning:
                toastService.ShowWarning(message);
                break;
            case Constraints.UI.MessageType.Information:
                toastService.ShowInfo(message);
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
        toastService.ShowCommunicationToast(new ToastParameters<CommunicationToastContent>()
        {
            Intent = t,
            Title = title,
            Content = new CommunicationToastContent()
            {
                Subtitle = message
            },
        });
    }

    public void Alert(MessageType type, string title, string message)
    {
        switch (type)
        {
            case MessageType.Success:
                dialogService.ShowSuccess(message, title);
                break;
            case MessageType.Warning:
                dialogService.ShowWarning(message, title);
                break;
            case MessageType.Error:
                dialogService.ShowError(message, title);
                break;
            case MessageType.Information:
                dialogService.ShowInfo(message, title);
                break;
            default:
                break;
        }
    }

    public async Task<bool> ConfirmAsync(string title, string message)
    {
        var localizer = ServiceProvider.GetService<IStringLocalizer<object>>()!;
        var r = await dialogService.ShowConfirmationAsync(message, localizer["CustomButtons.Ok"], localizer["CustomButtons.Cancel"], title);
        var dialogResult = await r.Result;
        return !dialogResult.Cancelled;
    }

    public async Task<TReturn> ShowDialogAsync<TReturn>(FlyoutOptions<TReturn> options)
    {
        TaskCompletionSource<TReturn> tcs = new();
        var localizer = ServiceProvider.GetService<IStringLocalizer<object>>()!;
        DialogParameters parameters = new();
        parameters.Title = options.Title;
        parameters.PrimaryAction = localizer[options.OkText ?? "CustomButtons.Ok"];
        parameters.SecondaryAction = localizer[options.CancelText ?? "CustomButtons.Cancel"];
        if (options.Width != null)
        {
            parameters.Width = options.Width;
        }
        parameters.OnDialogResult = EventCallback.Factory.Create<DialogResult>(this, async r =>
        {
            if (options.Feedback == null)
                return;
            if (r.Cancelled)
            {
                await options.Feedback.OnCancelAsync();
                return;
            }
            var result = await options.Feedback.OnOkAsync();
            if (result.Success)
            {
                tcs.TrySetResult(result.Value!);
            }
            else
            {
                tcs.TrySetCanceled();
            }
        });
        //parameters.
        var dialogRef = await dialogService.ShowDialogAsync(options.Content!, parameters);
        options.OnClose = dialogRef.CloseAsync;

        return await tcs.Task;
    }

    public async Task<TReturn> ShowDrawerAsync<TReturn>(FlyoutDrawerOptions<TReturn> options)
    {
        TaskCompletionSource<TReturn> tcs = new();
        var localizer = ServiceProvider.GetService<IStringLocalizer<object>>()!;
        DialogParameters parameters = new();
        parameters.Title = options.Title;
        parameters.PrimaryAction = localizer[options.OkText ?? "CustomButtons.Ok"];
        parameters.SecondaryAction = localizer[options.CancelText ?? "CustomButtons.Cancel"];
        if (options.Width != null)
        {
            parameters.Width = options.Width;
        }
        parameters.OnDialogClosing = EventCallback.Factory.Create<DialogInstance>(this, async i =>
        {
            var r = await options.Feedback!.OnOkAsync();
        });
        parameters.OnDialogResult = EventCallback.Factory.Create<DialogResult>(this, async r =>
        {
            if (options.Feedback == null)
                return;
            if (r.Cancelled)
            {
                await options.Feedback.OnCancelAsync();
                return;
            }
            var result = await options.Feedback.OnOkAsync();
            if (result.Success)
            {
                tcs.TrySetResult(result.Value!);
            }
            else
            {
                tcs.TrySetCanceled();
            }
        });
        //parameters.
        var dialogRef = await dialogService.ShowDialogAsync(options.Content!, parameters);
        await dialogRef.CloseAsync();
        var r = await dialogRef.Result;

        return await tcs.Task;
    }

    public IServiceProvider ServiceProvider { get; } = services;

    public IBindableInputComponent<DefaultProp, string> BuildInput(object receiver)
    {
        return new BindableComponentBuilder<FluentTextField, DefaultProp, string>() { Receiver = receiver };
    }

    public IBindableInputComponent<DefaultProp, string> BuildPassword(object receiver)
    {
        return new BindableComponentBuilder<FluentTextField, DefaultProp, string>(builder =>
        {
            builder.SetComponent(c => c.TextFieldType, TextFieldType.Password);
        })
        { Receiver = receiver };
    }

    public IBindableInputComponent<DefaultProp, TValue> BuildNumberInput<TValue>(object receiver) where TValue : new()
    {
        return new BindableComponentBuilder<FluentNumberField<TValue>, DefaultProp, TValue>() { Receiver = receiver };
    }

    public IBindableInputComponent<DatePickerProp, DateTime?> BuildDatePicker(object receiver)
    {
        return new BindableComponentBuilder<FluentDatePicker, DatePickerProp, DateTime?>() { Receiver = receiver };
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
            if (builder.Model.ButtonType == Constraints.UI.ButtonType.Default)
            {
                return;
            }
            switch (builder.Model.ButtonType)
            {
                case ButtonType.Primary:
                    builder.SetComponent(b => b.Appearance, Appearance.Accent);
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

    public RenderFragment BuildDropdown(DropdownOptions options)
    {
        return b => b.AddContent(1, "NotImplemented");
    }

    public RenderFragment BuildProfile()
    {
        return b => b.AddContent(1, "NotImplemented");
    }

    public RenderFragment BuildPopover(PopoverOptions options)
    {

        return b => b.AddContent(1, "NotImplemented");
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
        return new PropComponentBuilder<FluentDialog, ModalProp>().SetComponent(d => d.Hidden, true);
    }

    public IUIComponent<GridProp> BuildRow()
    {
        throw new NotImplementedException();
    }

    public IUIComponent<GridProp> BuildCol()
    {
        throw new NotImplementedException();
    }

    public IUIComponent<CardProp> BuildCard()
    {
        return new PropComponentBuilder<FluentCard, CardProp>(card =>
        {
            card.SetComponent(c => c.ChildContent, card.Model.ChildContent);
        });
    }

    public RenderFragment RenderContainer()
    {
        // <FluentToastProvider />
        // <FluentDialogProvider />
        // <FluentTooltipProvider />
        // <FluentMessageBarProvider />
        return b =>
        {
            b.Component<FluentToastProvider>().Build();
            b.Component<FluentDialogProvider>().Build();
            b.Component<FluentTooltipProvider>().Build();
            b.Component<FluentMessageBarProvider>().Build();
        };
    }

    public int GetMenuWidth(bool collapsed)
    {
        return collapsed ? 42 : 240;
    }


}