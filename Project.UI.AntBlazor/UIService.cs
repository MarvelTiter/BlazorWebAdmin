using AntDesign;
using Microsoft.AspNetCore.Components;
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
using Project.UI.AntBlazor.Components;
using System;
using System.Linq.Expressions;

namespace Project.UI.AntBlazor
{

    public class UIService(ModalService modalService, MessageService messageService, DrawerService drawerService, NotificationService notificationService) : IUIService
    {
        private readonly ModalService modalService = modalService;
        private readonly MessageService messageService = messageService;
        private readonly DrawerService drawerService = drawerService;
        private readonly NotificationService notificationService = notificationService;
        public IBindableInputComponent<DefaultProp, TValue> BuildInput<TValue>(object reciver)
        {
            return new BindableInputComponentBuilder<Input<TValue>, DefaultProp, TValue>() { Reciver = reciver };
        }

        public IBindableInputComponent<DefaultProp, TValue> BuildNumberInput<TValue>(object reciver)
        {
            return new BindableInputComponentBuilder<InputNumber<TValue>, DefaultProp, TValue>() { Reciver = reciver };
        }

        public IBindableInputComponent<DefaultProp, TValue> BuildDatePicker<TValue>(object reciver)
        {
            return new BindableInputComponentBuilder<DatePicker<TValue>, DefaultProp, TValue> { Reciver = reciver };
        }

        public IBindableInputComponent<DefaultProp, string> BuildPassword(object reciver)
        {
            return new BindableInputComponentBuilder<InputPassword, DefaultProp, string>() { Reciver = reciver };
        }

        public IBindableInputComponent<SelectProp, TValue> BuildSelect<TValue>(object reciver, SelectItem<TValue>? options)
        {
            if (typeof(TValue).IsEnum && options == null)
            {
                return new BindableInputComponentBuilder<EnumSelect<TValue>, SelectProp, TValue>() { Reciver = reciver };
            }
            else
            {
                return new BindableInputComponentBuilder<Select<TValue, Options<TValue>>, SelectProp, TValue>(self =>
                {
                    self.SetComponent(s => s.DataSource, options)
                    .SetComponent(s => s.ValueName, "Value")
                    .SetComponent(s => s.LabelName, "Label")
                    .SetComponent(s => s.AllowClear, self.Model.AllowClear)
                    .SetComponent(s => s.EnableSearch, self.Model.AllowSearch);
                })
                { Reciver = reciver };
            }
        }

        public ISelectInput<SelectProp, TItem, TValue> BuildSelect<TItem, TValue>(object reciver, IEnumerable<TItem> options)
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

            })
            { Reciver = reciver };

        }

        public IBindableInputComponent<SwitchProp, bool> BuildSwitch(object reciver)
        {
            // "CheckedChildren", "UnCheckedChildren"
            var binder = new BindableInputComponentBuilder<Switch, SwitchProp, bool>(self =>
            {
                self.SetComponent(sw => sw.CheckedChildren, self.Model.CheckedLabel)
                .SetComponent(sw => sw.UnCheckedChildren, self.Model.UnCheckedLabel);
            })
            { Reciver = reciver };
            // @bind-Check
            binder.Model.BindValueName = "Check";
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
                switch (self.Model.ButtonType)
                {
                    case Constraints.UI.ButtonType.Primary:
                        self.SetComponent(b => b.Type, "primary");
                        break;
                    case Constraints.UI.ButtonType.Danger:
                        self.SetComponent(b => b.Danger, true);
                        break;
                    default:
                        break;
                }

                self.TrySet("ChildContent", (RenderFragment)(builder => builder.AddContent(1, self.Model.Text)));

            })
            { Reciver = reciver };
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
                    break;
            }
        }

        public void Alert(Constraints.UI.MessageType type, string title, string message)
        {
            var option = new ConfirmOptions
            {
                Title = title,
                Content = message
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
                    break;
            }
        }

        public RenderFragment BuildTable<TModel, TQuery>(TableOptions<TModel, TQuery> options) where TQuery : IRequest, new()
        {
            return builder =>
            {
                builder.OpenComponent<AntTable<TModel, TQuery>>(0);
                builder.AddComponentParameter(1, nameof(AntTable<TModel, TQuery>.Options), options);
                builder.CloseComponent();
            };
        }

        public RenderFragment BuildTableHeader<TModel, TQuery>(TableOptions<TModel, TQuery> options) where TQuery : IRequest, new()
        {
            return builder =>
            {
                builder.OpenComponent<AntTableDefaultHeader<TModel, TQuery>>(0);
                builder.AddComponentParameter(1, nameof(AntTableDefaultHeader<TModel, TQuery>.Options), options);
                builder.CloseComponent();
            };
        }

        public RenderFragment BuildForm<TData>(FormOptions<TData> options) where TData : class, new()
        {
            return builder =>
            {
                builder.OpenComponent<AntForm<TData>>(0);
                builder.AddComponentParameter(1, nameof(AntForm<TData>.Options), options);
                builder.CloseComponent();
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
            return builder =>
            {
                builder.OpenComponent<AntDropdown>(0);
                builder.AddComponentParameter(1, nameof(AntDropdown.Options), options);
                builder.CloseComponent();
            };
        }

        public RenderFragment BuildMenu(IRouterStore router, bool horizontal, IAppStore app)
        {
            return builder =>
            {
                builder.OpenComponent<AntMenu>(0);
                builder.AddComponentParameter(1, nameof(AntMenu.Router), router);
                builder.AddComponentParameter(2, nameof(AntMenu.Horizontal), horizontal);
                builder.AddComponentParameter(3, nameof(AntMenu.App), app);
                builder.CloseComponent();
            };
        }

        public RenderFragment BuildLoginForm(LoginFormModel model, Func<Task> handleLogin)
        {
            return builder =>
            {
                builder.OpenComponent<AntLogin>(0);
                builder.AddComponentParameter(1, nameof(AntLogin.LoginModel), model);
                builder.AddComponentParameter(2, nameof(AntLogin.HandleLogin), handleLogin);
                builder.CloseComponent();
            };
        }

        public async Task<TReturn> ShowDialogAsync<TReturn>(FlyoutOptions<TReturn> options)
        {
            TaskCompletionSource<TReturn> tcs = new();
            var modal = new ModalOptions
            {
                Title = options.Title,
                Content = options.Content,
                DestroyOnClose = true,
                OkText = "确定",
                CancelText = "取消",
                Maximizable = true,
                OnOk = e => options.OnOk.Invoke(),
                OnCancel = e => options.OnClose.Invoke()
            };
            if (options.Width != null) modal.Width = options.Width;
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

        public IUIComponent BuildRow()
        {
            return new ComponentBuilder<Row>();
        }

        public IUIComponent BuildCol()
        {
            return new ComponentBuilder<AntCol>();
        }

        public IBindableInputComponent<DefaultProp, bool> BuildCheckBox(object reciver)
        {
            var binder = new BindableInputComponentBuilder<Checkbox, DefaultProp, bool>() { Reciver = reciver };
            binder.Model.BindValueName = "Checked";
            return binder;
        }

        public IBindableInputComponent<DefaultProp, string[]> BuildTree<TData>(object reciver, TreeOptions<TData> options)
        {
            //return builder =>
            //{
            //    builder.OpenComponent<AntTree<TData>>(0);
            //    builder.AddAttribute(1, nameof(AntTree<TData>.Options), options);
            //    builder.CloseComponent();
            //};
            var builder = new BindableInputComponentBuilder<AntTree<TData>, DefaultProp, string[]>()
            {
                Reciver = reciver
            };
            builder.SetComponent(t => t.Options, options);
            builder.Model.BindValueName = "CheckedKeys";
            builder.Model.EnableValueExpression = false;
            return builder;
        }
    }
}
