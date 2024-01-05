using AntDesign;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Options;
using Microsoft.VisualBasic.FileIO;
using Project.Constraints.Store;
using Project.Constraints.UI;
using Project.Constraints.UI.Dropdown;
using Project.Constraints.UI.Flyout;
using Project.Constraints.UI.Form;
using Project.Constraints.UI.Table;
using Project.Models;
using Project.Models.Forms;
using Project.Models.Request;
using Project.UI.AntBlazor.Components;
using System.Reflection;

namespace Project.UI.AntBlazor
{

    public class UIService(ModalService modalService, MessageService messageService, DrawerService drawerService) : IUIService
    {
        private readonly ModalService modalService = modalService;
        private readonly MessageService messageService = messageService;
        private readonly DrawerService drawerService = drawerService;


        public IBindableInput<string> BuildInput(object reciver)
        {
            return new BindableComponentBuilder<Input<string>, string>() { Reciver = reciver };
        }

        public IBindableInput<TValue> BuildInput<TValue>(object reciver)
        {
            return new BindableComponentBuilder<InputNumber<TValue>, TValue>() { Reciver = reciver };
        }

        public IBindableInput<TValue> BuildDatePicker<TValue>(object reciver)
        {
            return new BindableComponentBuilder<DatePicker<TValue>, TValue> { Reciver = reciver };
        }

        public IBindableInput<string> BuildPassword(object reciver)
        {
            return new BindableComponentBuilder<InputPassword, string>() { Reciver = reciver };
        }

        public IBindableInput<TValue> BuildSelect<TValue>(object reciver, SelectItem<TValue>? options)
        {
            if (typeof(TValue).IsEnum && options == null)
            {
                return new BindableComponentBuilder<EnumSelect<TValue>, TValue>() { Reciver = reciver };
            }
            else
            {
                return new BindableComponentBuilder<Select<TValue, Options<TValue>>, TValue>() { Reciver = reciver }
                    .Set("DataSource", options!)
                    .Set("ValueName", "Value")
                    .Set("LabelName", "Label");
            }
        }

        public ISelectInput<TItem, TValue> BuildSelect<TItem, TValue>(object reciver, IEnumerable<TItem> options)
        {
            return new SelectBuilder<Select<TValue, TItem>, TItem, TValue>() { Reciver = reciver }
            .Set("DataSource", options);
        }

        public IBindableInput<bool> BuildSwitch(object reciver)
        {
            return new BindableComponentBuilder<Switch, bool>() { Reciver = reciver };
        }

        public IButtonAction BuildButton(object reciver)
        {
            return new ButtonBuilder<Button>((self,p) =>
            {
                if (self.ButtonType == Constraints.UI.ButtonType.Default)
                {
                    return;
                }
                switch (self.ButtonType)
                {
                    case Constraints.UI.ButtonType.Primary:
                        p.Add("Type", "primary");
                        break;
                    case Constraints.UI.ButtonType.Danger:
                        p.Add("Danger", true);
                        break;
                    default:
                        break;
                }
            }) { Reciver = reciver };
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

        public IBindableInput<bool> BuildCheckBox(object reciver)
        {
            return new BindableComponentBuilder<Checkbox, bool>() { Reciver = reciver };
        }


    }
}
