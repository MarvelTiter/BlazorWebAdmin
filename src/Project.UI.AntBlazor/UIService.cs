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

namespace Project.UI.AntBlazor
{
    public class UIService(
        ModalService modalService,
        MessageService messageService,
        DrawerService drawerService,
        NotificationService notificationService,
        IServiceProvider services) : IUIService
    {
        private readonly ModalService modalService = modalService;
        private readonly MessageService messageService = messageService;
        private readonly DrawerService drawerService = drawerService;
        private readonly NotificationService notificationService = notificationService;

        public IServiceProvider ServiceProvider { get; } = services;

        public IBindableInputComponent<DefaultProp, TValue> BuildInput<TValue>(object reciver)
        {
            return new BindableComponentBuilder<Input<TValue>, DefaultProp, TValue>() { Receiver = reciver };
        }

        public IBindableInputComponent<DefaultProp, TValue> BuildNumberInput<TValue>(object reciver)
        {
            return new BindableComponentBuilder<InputNumber<TValue>, DefaultProp, TValue>() { Receiver = reciver };
        }

        public IBindableInputComponent<DefaultProp, TValue> BuildDatePicker<TValue>(object reciver)
        {
            return new BindableComponentBuilder<DatePicker<TValue>, DefaultProp, TValue> { Receiver = reciver };
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
                return new BindableComponentBuilder<EnumSelect<TValue>, SelectProp, TValue>() { Receiver = reciver };
            }
            else
            {
                return new BindableComponentBuilder<Select<TValue, Options<TValue>>, SelectProp, TValue>(self =>
                    {
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
                    self.SetComponent(s => s.DataSource, options);
                    if (self.Model.ValueExpression is LambdaExpression valueLambda)
                        self.SetComponent(s => s.ValueProperty, valueLambda.Compile());
                    if (self.Model.LabelExpression is LambdaExpression labelLambda)
                        self.SetComponent(s => s.LabelProperty, labelLambda.Compile());

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
                        case { Model: { ButtonType: Constraints.UI.ButtonType.Primary } }:
                            self.SetComponent(b => b.Type, "primary");
                            break;
                        case { Model: { ButtonType: Constraints.UI.ButtonType.Danger } }:
                            self.SetComponent(b => b.Danger, true);
                            break;
                    }

                    self.TrySet("ChildContent", (RenderFragment)(builder => builder.AddContent(1, self.Model.Text)));
                })
            { Receiver = reciver };
        }

        public void Message(Constraints.UI.MessageType type, string message)
        {
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

        public RenderFragment BuildProfile(ProfileInfo info)
        {
            return builder => builder.Component<AntProfile>().SetComponent(c => c.Options, info).Build();
        }

        public RenderFragment BuildMenu(IRouterStore router, bool horizontal, IAppStore app)
        {
            return builder => builder.Component<AntMenu>()
                    .SetComponent(c => c.Router, router)
                    .SetComponent(c => c.Horizontal, horizontal)
                    .SetComponent(c => c.App, app)
                    .Build();
        }

        public RenderFragment BuildLoginForm(LoginFormModel model, Func<Task> handleLogin)
        {
            return builder =>
            {
                builder.Component<AntLogin>()
                    .SetComponent(c => c.LoginModel, model)
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
                OnOk = e => options.OnOk.Invoke(),
                OnCancel = e => options.OnClose.Invoke(),
            };

            if (options.Width != null) modal.Width = options.Width;
            if (options.ShowFooter && options.Footer != null) modal.Footer = options.Footer;
            else if (!options.ShowFooter) modal.Footer = null;

            var modalRef = await modalService.CreateModalAsync(modal);
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
                Placement = options.Position.ToString().ToLower(),
            };
            if (options.Width != null) modal.Width = int.Parse(options.Width);

            _ = await drawerService.CreateAsync(modal);

            return default;
        }

        public IUIComponent BuildCard()
        {
            return new ComponentBuilder<Card>();
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
            var binder = new BindableComponentBuilder<Checkbox, DefaultProp, bool>() { Receiver = reciver };
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
            return new SelectComponentBuilder<CheckboxGroup, SelectProp, TItem, TValue[]>(self =>
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
                })
            { Receiver = reciver };
        }

        public RenderFragment BuildPopover(PopoverOptions options)
        {
            return builder => { builder.Component<AntPopover>().SetComponent(c => c.Options, options).Build(); };
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
    }
}