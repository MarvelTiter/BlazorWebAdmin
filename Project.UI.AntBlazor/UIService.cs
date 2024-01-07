using AntDesign;
using Microsoft.AspNetCore.Components;
using Project.Constraints.Store;
using Project.Constraints.UI;
using Project.Constraints.UI.Builders;
using Project.Constraints.UI.Dropdown;
using Project.Constraints.UI.Flyout;
using Project.Constraints.UI.Form;
using Project.Constraints.UI.Props;
using Project.Constraints.UI.Table;
using Project.Models;
using Project.Models.Forms;
using Project.Models.Request;
using Project.UI.AntBlazor.Components;
using System.Linq.Expressions;

namespace Project.UI.AntBlazor
{

    public class UIService(ModalService modalService, MessageService messageService, DrawerService drawerService) : IUIService
    {
        private readonly ModalService modalService = modalService;
        private readonly MessageService messageService = messageService;
        private readonly DrawerService drawerService = drawerService;


        public IBindableInputComponent<EmptyProp, TValue> BuildInput<TValue>(object reciver)
        {
            return new BindableInputComponentBuilder<Input<TValue>, EmptyProp, TValue>() { Reciver = reciver };
        }

        public IBindableInputComponent<EmptyProp, TValue> BuildNumberInput<TValue>(object reciver)
        {
            return new BindableInputComponentBuilder<InputNumber<TValue>, EmptyProp, TValue>() { Reciver = reciver };
        }

        public IBindableInputComponent<EmptyProp, TValue> BuildDatePicker<TValue>(object reciver)
        {
            return new BindableInputComponentBuilder<DatePicker<TValue>, EmptyProp, TValue> { Reciver = reciver };
        }

        public IBindableInputComponent<EmptyProp, string> BuildPassword(object reciver)
        {
            return new BindableInputComponentBuilder<InputPassword, EmptyProp, string>() { Reciver = reciver };
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
                    .SetComponent(s => s.LabelName, "Label");
                })
                { Reciver = reciver };
            }
        }

        public ISelectInput<SelectProp, TItem, TValue> BuildSelect<TItem, TValue>(object reciver, IEnumerable<TItem> options)
        {
            return new SelectComponentBuilder<Select<TValue, TItem>, SelectProp, TItem, TValue>(self =>
            {
                self.SetComponent(s=>s.DataSource, options);
                if (self.Model.ValueExpression is LambdaExpression valueLambda)
                    self.SetComponent(s => s.ValueProperty, valueLambda.Compile());
                if (self.Model.LabelExpression is LambdaExpression labelLambda)
                    self.SetComponent(s => s.LabelProperty, labelLambda.Compile());
            })
            { Reciver = reciver };

        }

        public IBindableInputComponent<EmptyProp, bool> BuildSwitch(object reciver)
        {
            return new BindableInputComponentBuilder<Switch, EmptyProp, bool>() { Reciver = reciver };
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



        public void Notify(Constraints.UI.MessageType type, string title, string message)
        {
            throw new NotImplementedException();
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

        public IBindableInputComponent<EmptyProp, bool> BuildCheckBox(object reciver)
        {
            return new BindableInputComponentBuilder<Checkbox, EmptyProp, bool>() { Reciver = reciver };
        }


    }
}
