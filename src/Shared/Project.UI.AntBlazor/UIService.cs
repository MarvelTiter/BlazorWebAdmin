using AntDesign;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Project.Constraints.Models;
using Project.Constraints.Models.Request;
using Project.Constraints.Store;
using Project.Constraints.UI;
using Project.Constraints.UI.Builders;
using Project.Constraints.UI.Dropdown;
using Project.Constraints.UI.Extensions;
using Project.Constraints.UI.Flyout;
using Project.Constraints.UI.Form;
using Project.Constraints.UI.Props;
using Project.Constraints.UI.Table;
using Project.Constraints.UI.Tree;
using Project.UI.AntBlazor.Components;
using System.Linq.Expressions;
using OneOf;
using Microsoft.Extensions.DependencyInjection;
using AutoInjectGenerator;
using Project.Web.Shared.Components;
using Microsoft.JSInterop;
using Project.UI.AntBlazor.WebSettings;
using Project.Web.Shared.Utils;

namespace Project.UI.AntBlazor;

[AutoInject]
public class UIService(
    ModalService modalService,
    IMessageService messageService,
    DrawerService drawerService,
    INotificationService notificationService,
    IServiceProvider services,
    IJSRuntime js,
    IAppStore app) : IUIService
{
    public IServiceProvider ServiceProvider { get; } = services;
    private Action? update;
    public void RegisterUpdateUIAction(Action updateUI)
    {
        update ??= updateUI;
    }
    public string MainStyle() => "_content/AntDesign/css/ant-design-blazor.css";
    internal static readonly ThemeMode CompactMode = new("Compact", 3);
    public RenderFragment AddStyles()
    {
        return b =>
        {
            //b.Component<VLink>().SetComponent(c => c.Href, "_content/AntDesign/css/ant-design-blazor.css").Build();
            b.Component<VLink>().SetComponent(c => c.Href, "_content/AntDesign/css/ant-design-blazor.variable.css").Build();
            b.OpenElement(0, "link");
            b.AddAttribute(1, "rel", "stylesheet");
            b.AddAttribute(2, "data-dark");
            b.CloseElement();
            b.OpenElement(0, "link");
            b.AddAttribute(1, "rel", "stylesheet");
            b.AddAttribute(2, "data-compact");
            b.CloseElement();
            b.Component<VLink>().SetComponent(c => c.Href, "_content/Project.UI.AntBlazor/ant.css").Build();
        };
    }
    //_content/AntDesign/css/ant-design-blazor.compact.css
    //_content/AntDesign/css/ant-design-blazor.dark.css
    //_content/AntDesign/css/ant-design-blazor.variable.css
    public const string DARK_CSS = "_content/AntDesign/css/ant-design-blazor.dark.css";
    public const string COMPACT_CSS = "_content/AntDesign/css/ant-design-blazor.compact.css";
    public string DarkStyle() => "_content/AntDesign/css/ant-design-blazor.dark.css";

    public async Task OnAppMounted(IAppStore app)
    {
        if (app.Theme == CompactMode)
        {
            await js.InvokeVoidAsync("setCompactStyleSheet", COMPACT_CSS);
        }
        else
        {
            await js.InvokeVoidAsync("setTheme", $"{app.Theme}".ToLower(), DARK_CSS);
            await js.InvokeVoidAsync("setDark");
        }
    }

    public RenderFragment UIFrameworkJs()
    {
        return b =>
        {
            b.Component<VScript>().SetComponent(c => c.Src, "_content/AntDesign/js/ant-design-blazor.js").Build();
            b.Component<VScript>().SetComponent(c => c.Src, "_content/Project.UI.AntBlazor/ant-color.js").Build();
            b.Component<VScript>().SetComponent(c => c.Src, "_content/Project.UI.AntBlazor/ant-style.js").Build();
            //b.Component<VScript>().SetComponent(c => c.Src, "_content/Project.UI.AntBlazor/less.min.js").Build();
        };
    }

    //public RenderFragment BuildIcon(string name)
    //{
    //    return builder => builder.Component<Icon>().SetComponent(c => c.Type, name).Build();
    //}

    public IBindableInputComponent<DefaultProp, string> BuildInput(object reciver)
    {
        return new BindableComponentBuilder<Input<string>, DefaultProp, string>() { Receiver = reciver };
    }

    public IBindableInputComponent<DefaultProp, TValue> BuildNumberInput<TValue>(object reciver) where TValue : new()
    {
        return new BindableComponentBuilder<InputNumber<TValue>, DefaultProp, TValue>() { Receiver = reciver };
    }

    public IBindableInputComponent<DatePickerProp, DateTime?> BuildDatePicker(object reciver)
    {
        return new BindableComponentBuilder<DatePicker<DateTime?>, DatePickerProp, DateTime?>(builder =>
            {
                if (builder.Model.WithTime)
                {
                    builder.SetComponent(c => c.ShowTime, "HH:mm");
                }
            })
        { Receiver = reciver };
    }

    public IBindableInputComponent<DefaultProp, string> BuildPassword(object reciver)
    {
        return new BindableComponentBuilder<InputPassword, DefaultProp, string>() { Receiver = reciver };
    }

    public IBindableInputComponent<SelectProp, TValue> BuildSelect<TValue>(object reciver,
        SelectItem<TValue>? options)
    {
        if (typeof(TValue).IsEnum && options == null)
        {
            return new BindableComponentBuilder<EnumSelect<TValue>, SelectProp, TValue>(self =>
            {
                if (self.Model.Mulitple)
                {
                    self.SetComponent(s => s.Mode, SelectMode.Multiple);
                    self.Model.BindValueName = "Values";
                    self.Model.ValueExpressionName = "ValuesExpression";
                }
            })
            { Receiver = reciver };
        }
        else
        {
            return new BindableComponentBuilder<Select<TValue, Options<TValue>>, SelectProp, TValue>(self =>
                {
                    if (self.Model.Mulitple)
                    {
                        self.SetComponent(s => s.Mode, SelectMode.Multiple);
                        self.Model.BindValueName = "Values";
                        self.Model.ValueExpressionName = "ValuesExpression";
                    }
                    self.SetComponent(s => s.DataSource, options)
                        .SetComponent(s => s.ValueName, "Value")
                        .SetComponent(s => s.LabelName, "Label")
                        .SetComponent(s => s.DropdownMatchSelectWidth, false)
                        .SetComponent(s => s.AllowClear, self.Model.AllowClear)
                        .SetComponent(s => s.EnableSearch, self.Model.AllowSearch);
                })
            { Receiver = reciver };
        }
    }

    public ISelectInput<SelectProp, TItem, TValue> BuildSelect<TItem, TValue>(object reciver,
        IEnumerable<TItem> options)
    {
        return new SelectComponentBuilder<Select<TValue, TItem>, SelectProp, TItem, TValue>(self =>
            {
                if (self.Model.Mulitple)
                {
                    self.SetComponent(s => s.Mode, SelectMode.Multiple);
                    self.Model.BindValueName = "Values";
                    self.Model.ValueExpressionName = "ValuesExpression";
                }
                self.SetComponent(s => s.DataSource, options);
                if (self.Model.ValueExpression is LambdaExpression valueLambda)
                    self.SetComponent(s => s.ItemValue, valueLambda.Compile());
                if (self.Model.LabelExpression is LambdaExpression labelLambda)
                    self.SetComponent(s => s.ItemLabel, labelLambda.Compile());

                self.SetComponent(s => s.AllowClear, self.Model.AllowClear);
                self.SetComponent(s => s.EnableSearch, self.Model.AllowSearch);
                self.SetComponent(s => s.DropdownMatchSelectWidth, false);
            })
        { Receiver = reciver };
    }

    public IBindableInputComponent<SwitchProp, bool> BuildSwitch(object reciver)
    {
        // "CheckedChildren", "UnCheckedChildren"
        var binder = new BindableComponentBuilder<Switch, SwitchProp, bool>(self =>
            {
                self.SetComponent(sw => sw.CheckedChildren, self.Model.CheckedLabel)
                    .SetComponent(sw => sw.UnCheckedChildren, self.Model.UnCheckedLabel);
            })
        { Receiver = reciver };
        // @bind-Check
        binder.Model.BindValueName = "Checked";
        return binder;
    }

    public IButtonInput BuildButton(object reciver)
    {
        return new ButtonComponentBuilder<Button>((self) =>
            {
                if (self.Model.ButtonType == Constraints.UI.ButtonType.Default)
                {
                    return;
                }

                switch (self)
                {
                    case { Model.ButtonType: Constraints.UI.ButtonType.Primary }:
                        self.SetComponent(b => b.Type, AntDesign.ButtonType.Primary);
                        break;
                    case { Model.ButtonType: Constraints.UI.ButtonType.Danger }:
                        self.SetComponent(b => b.Danger, true);
                        break;
                }

                self.TrySet("ChildContent", (RenderFragment)(builder => builder.AddContent(1, self.Model.Text)));

            })
        { Receiver = reciver };
    }

    public RenderFragment BuildFakeButton(ButtonProp props)
    {
        var tyleClass = props.ButtonType switch
        {
            Constraints.UI.ButtonType.Primary => "ant-btn-primary",
            Constraints.UI.ButtonType.Danger => "ant-btn-dangerous",
            _ => "ant-btn-default"
        };
        var spanClass = $"ant-btn {tyleClass}";
        return b => b.Span().Set("class", spanClass).AddText(props.Text).Build();
    }

    public void Message(Constraints.UI.MessageType type, string message)
    {
        // var messageConfig = new MessageConfig();
        // messageConfig.
        switch (type)
        {
            case Constraints.UI.MessageType.Success:
                messageService.Success(message);
                break;
            case Constraints.UI.MessageType.Error:
                messageService.Error(message);
                break;
            case Constraints.UI.MessageType.Warning:
                messageService.Warning(message);
                break;
            case Constraints.UI.MessageType.Information:
                messageService.Info(message);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }

    public void Alert(Constraints.UI.MessageType type, string title, string message)
    {
        var option = new ConfirmOptions
        {
            Title = title,
            Content = message,
            Centered = true,
        };
        switch (type)
        {
            case Constraints.UI.MessageType.Success:
                modalService.Success(option);
                break;
            case Constraints.UI.MessageType.Error:
                modalService.Error(option);
                break;
            case Constraints.UI.MessageType.Warning:
                modalService.Warning(option);
                break;
            case Constraints.UI.MessageType.Information:
                modalService.Info(option);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }

    public Task<bool> ConfirmAsync(string title, string message)
    {
        var option = new ConfirmOptions
        {
            Title = title,
            Content = message,
            Centered = true,
        };
        return modalService.ConfirmAsync(option);
    }

    public RenderFragment BuildTable<TModel, TQuery>(TableOptions<TModel, TQuery> options)
        where TQuery : IRequest, new()
    {
        return builder =>
        {
            builder.Component<AntTable<TModel, TQuery>>()
                .SetComponent(c => c.Options, options)
                .Build();
        };
    }

    public RenderFragment BuildDynamicTable<TRowData, TQuery>(TableOptions<TRowData, TQuery> options)
        where TQuery : IRequest, new()
    {
        return builder =>
        {
            builder.Component<AntDynamicTable<TRowData, TQuery>>()
                .SetComponent(c => c.Options, options)
                .Build();
        };
    }

    public RenderFragment BuildForm<TData>(FormOptions<TData> options) where TData : class, new()
    {
        return builder =>
        {
            builder.Component<AntForm<TData>>()
                .SetComponent(c => c.Options, options)
                .Build();
        };
    }


    // TODO Notify
    public void Notify(Constraints.UI.MessageType type, string title, string message)
    {
        _ = notificationService.Error(new NotificationConfig()
        {
            Message = title,
            Description = message,
            Placement = NotificationPlacement.BottomRight,
        });
    }

    public RenderFragment BuildDropdown(DropdownOptions options)
    {
        return builder => builder.Component<AntDropdown>()
            .SetComponent(c => c.Options, options)
            .Build();
    }

    public RenderFragment BuildProfile()
    {
        return builder => builder.Component<AntProfile>().Build();
    }

    public RenderFragment BuildMenu(IRouterStore router, bool horizontal, IAppStore app)
    {
        return builder => builder.Component<AntMenu>()
            .SetComponent(c => c.Router, router)
            .SetComponent(c => c.Horizontal, horizontal)
            .SetComponent(c => c.App, app)
            .Build();
    }

    public RenderFragment BuildLoginForm(Func<LoginFormModel, Task> handleLogin)
    {
        return builder =>
        {
            builder.Component<AntLogin>()
                .SetComponent(c => c.HandleLogin, handleLogin)
                .Build();
        };
    }

    public async Task<TReturn> ShowDialogAsync<TReturn>(FlyoutOptions<TReturn> options)
    {
        var localizer = ServiceProvider.GetRequiredService<IStringLocalizer<object>>();
        TaskCompletionSource<TReturn> tcs = new();
        var modal = new ModalOptions
        {
            Title = options.Title,
            Content = options.Content,
            DestroyOnClose = true,
            OkText = localizer[options.OkText ?? "CustomButtons.Ok"].Value,
            CancelText = localizer[options.CancelText ?? "CustomButtons.Cancel"].Value,
            Maximizable = true,
            Draggable = true,
            DragInViewport = true,
            OnOk = e => options.OnOk?.Invoke(),
            OnCancel = e => options.OnClose?.Invoke(),
            DefaultMaximized = options.FullScreen
        };

        if (options.Width != null) modal.Width = options.Width;
        if (options.Top != null) modal.Style = $"top: {options.Top}";
        if (options.ShowFooter && options.Footer != null) modal.Footer = options.Footer;
        else if (!options.ShowFooter) modal.Footer = null;

        var modalRef = modalService.CreateModal(modal);
        options.OnClose = async () =>
        {
            if (options.Feedback != null)
            {
                await options.Feedback.OnCancelAsync();
            }

            await modalRef.CloseAsync();
            tcs.TrySetCanceled();
        };
        options.OnOk = async () =>
        {
            if (options.Feedback != null)
            {
                var result = await options.Feedback.OnOkAsync();
                if (result.Success)
                {
                    tcs.TrySetResult(result.Value!);
                    await modalRef.OkAsync(null);
                }
            }
            else
            {
                tcs.TrySetCanceled();
                await modalRef.OkAsync(null);
            }
        };
        return await tcs.Task;
    }

    public async Task<TReturn> ShowDrawerAsync<TReturn>(FlyoutDrawerOptions<TReturn> options)
    {
        var modal = new DrawerOptions
        {
            Title = options.Title,
            ChildContent = options.Content,
            Placement = C(options.Position),
        };
        if (options.Width != null) modal.Width = options.Width;

        _ = await drawerService.CreateAsync(modal);

        return default!;

        static DrawerPlacement C(Position position)
        {
            return position switch
            {
                Position.Left => DrawerPlacement.Left,
                Position.Right => DrawerPlacement.Right,
                Position.Top => DrawerPlacement.Top,
                Position.Bottom => DrawerPlacement.Bottom,
                _ => throw new ArgumentOutOfRangeException(nameof(position), position, null)
            };
        }

    }

    public IUIComponent<CardProp> BuildCard()
    {
        return new PropComponentBuilder<Card, CardProp>(card =>
        {
            var title = card.Model.TitleTemplate ?? (string.IsNullOrEmpty(card.Model.Title) ? null : card.Model.Title.AsContent());
            if (title != null)
                card.SetComponent(c => c.TitleTemplate, title);
            card.SetComponent(c => c.ChildContent, card.Model.ChildContent);
        });
    }

    public IUIComponent<GridProp> BuildRow()
    {
        return new PropComponentBuilder<Row, GridProp>(row =>
        {
            row.SetComponent(c => c.ChildContent, row.Model.ChildContent);
        });
    }

    public IUIComponent<GridProp> BuildCol()
    {
        return new PropComponentBuilder<Col, GridProp>(col =>
        {
            col.SetComponent(c => c.ChildContent, col.Model.ChildContent)
                .SetComponent(c => c.Span, (OneOf<string, int>)col.Model.ColSpan);
        });
    }

    public IBindableInputComponent<DefaultProp, bool> BuildCheckBox(object reciver)
    {
        var binder = new BindableComponentBuilder<Checkbox, DefaultProp, bool>(self =>
            {
                if (self.Model.Label != null)
                    self.SetComponent(cb => cb.ChildContent, self.Model.Label.AsContent());
            })
        { Receiver = reciver };
        binder.Model.BindValueName = "Checked";
        return binder;
    }

    public IBindableInputComponent<DefaultProp, string[]> BuildTree<TData>(object reciver,
        TreeOptions<TData> options)
    {
        var builder = new BindableComponentBuilder<AntTree<TData>, DefaultProp, string[]>()
        {
            Receiver = reciver
        };
        builder.SetComponent(t => t.Options, options);
        builder.Model.BindValueName = "CheckedKeys";
        builder.Model.EnableValueExpression = false;
        return builder;
    }

    // TODO 绑定有BUG
    public ISelectInput<SelectProp, TItem, TValue[]> BuildCheckBoxGroup<TItem, TValue>(object reciver,
        IEnumerable<TItem> options)
    {
        return new SelectComponentBuilder<CheckboxGroup<TValue>, SelectProp, TItem, TValue[]>(self =>
            {
                // (OneOf<CheckboxOption[], string[]>)
                if (self.Model.LabelExpression is LambdaExpression labelLambda &&
                    self.Model.ValueExpression is LambdaExpression valueLambda)
                {
                    var label = (Func<TItem, string>)labelLambda.Compile();
                    var value = (Func<TItem, TValue>)valueLambda.Compile();
                    self.SetComponent(cbg => cbg.Options, options.ConvertToCheckBoxOptions(label, value));
                }
            })
        { Receiver = reciver };
    }

    public ISelectInput<SelectProp, TItem, TValue> BuildRadioGroup<TItem, TValue>(object reciver,
        IEnumerable<TItem> options)
    {
        return new SelectComponentBuilder<RadioGroup<TValue>, SelectProp, TItem, TValue>(self =>
            {
                if (self.Model.LabelExpression is LambdaExpression labelLambda &&
                    self.Model.ValueExpression is LambdaExpression valueLambda)
                {
                    var label = (Func<TItem, string>)labelLambda.Compile();
                    var value = (Func<TItem, TValue>)valueLambda.Compile();
                    self.SetComponent(rg => rg.Options, options.ConvertToRadioOptions(label, value));
                }

                if (self.Model.ButtonGroup)
                {
                    self.SetComponent(rg => rg.ButtonStyle, RadioButtonStyle.Solid);
                }
            })
        { Receiver = reciver };
    }

    public RenderFragment BuildPopover(PopoverOptions options)
    {
        return builder => { builder.Component<AntPopover>().SetComponent(c => c.Options, options).Build(); };
    }

    public IUIComponent<PopoverOptions> BuildPopover()
    {
        return new PropComponentBuilder<AntPopover, PopoverOptions>(
            self =>
            {
                self.SetComponent(p => p.Options, self.Model);
            });
    }

    public IUIComponent<ModalProp> BuildModal()
    {
        return new PropComponentBuilder<Modal, ModalProp>(self =>
        {
            self.SetComponent(m => m.Title, self.Model.Title)
                .SetComponent(m => m.Visible, self.Model.Visible)
                .SetComponent(m => m.VisibleChanged, self.Model.VisibleChanged)
                .SetComponent(m => m.ChildContent, self.Model.ChildContent);
            if (self.Model.HideDefaultFooter)
            {
                self.SetComponent(m => m.Footer, null);
            }

            if (!string.IsNullOrEmpty(self.Model.Width))
            {
                self.SetComponent(m => m.Width, self.Model.Width);
            }
        });
    }

    public RenderFragment RenderContainer()
    {
        return b => b.Component<AntContainer>().Build();
    }

    public IUIComponent<TabsProp> BuildTabs()
    {
        return new PropComponentBuilder<Tabs, TabsProp>(self =>
        {
            void tabContent(Microsoft.AspNetCore.Components.Rendering.RenderTreeBuilder b)
            {
                foreach (var item in self.Model.TabContents)
                {
                    b.Component<TabPane>()
                        .SetComponent(p => p.Tab, item.Title)
                        .SetComponent(p => p.TabTemplate, item.TitleTemplate)
                        .SetContent(item.Content ?? "".AsContent()).Build();
                }
            }
            self.SetComponent(m => m.ChildContent, tabContent);
        });
    }

    public RenderFragment RenderIcon(IconInfo icon)
    {
        return b => b.Component<Icon>()
            .SetComponent(i => i.Type, icon.Name)
            .SetComponent(i => i.Class, icon.Class)
            .Build();
    }

    public int GetMenuWidth(bool collapsed)
    {
        return collapsed ? 80 : app.SideBarExpandWidth;
    }

    public IEnumerable<WebSettingFragment> WebSettings()
    {
        IconService.GetAllIcons();
        //yield return new WebSettingFragment("系统主题", b => b.Component<AntThemeManager>().Build());
        //yield return new WebSettingFragment("菜单主题", b => b.Component<AntMenuSetting>().Build());
        //yield return new("主题设置", b => b.Component<AntColorSetting>().Build());
        yield return new("主题设置", b => b.Component<AntSetting>().Build());
    }
}