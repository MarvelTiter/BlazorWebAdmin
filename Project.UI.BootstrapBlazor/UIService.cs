using BootstrapBlazor.Components;
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

namespace Project.UI.BootstrapBlazor
{
    public class UIService(
        DialogService dialogService,
        MessageService messageService,
        NotificationService notificationService,
        IServiceProvider provider
        ) : IUIService
    {
        private readonly DialogService dialogService = dialogService;
        private readonly MessageService messageService = messageService;
        private readonly NotificationService notificationService = notificationService;

        public IServiceProvider ServiceProvider => provider;

        public void Alert(MessageType type, string title, string message)
        {
            NotificationItem item = new NotificationItem();
            item.Title = title;
            item.Message = message;
            _ = notificationService.Dispatch(item);
        }

        public IButtonInput BuildButton(object receiver)
        {
            return new ButtonComponentBuilder<Button>(self =>
            {
                self.SetComponent(b => b.Text, self.Model.Text);
                switch (self.Model.ButtonType)
                {
                    case Constraints.UI.ButtonType.Primary:
                        self.SetComponent(b => b.Color, Color.Primary);
                        break;
                    case Constraints.UI.ButtonType.Secondary:
                        self.SetComponent(b => b.Color, Color.Secondary);
                        break;
                    case Constraints.UI.ButtonType.Danger:
                        self.SetComponent(b => b.Color, Color.Danger);
                        break;
                    case Constraints.UI.ButtonType.Success:
                        self.SetComponent(b => b.Color, Color.Success);
                        break;
                    default:
                        break;
                }
            })
            { Receiver = receiver };
        }

        public IUIComponent BuildCard()
        {
            return new ComponentBuilder<Card>();
        }

        public IBindableInputComponent<DefaultProp, bool> BuildCheckBox(object receiver)
        {
            var binder = new BindableComponentBuilder<Checkbox<bool>, DefaultProp, bool>() { Receiver = receiver };
            return binder;
        }

        public ISelectInput<SelectProp, TItem, TValue[]> BuildCheckBoxGroup<TItem, TValue>(object receiver, IEnumerable<TItem> options)
        {
            throw new NotImplementedException();
        }

        public IUIComponent BuildCol()
        {
            throw new NotImplementedException();
        }

        public IBindableInputComponent<DefaultProp, TValue> BuildDatePicker<TValue>(object receiver)
        {
            return new BindableComponentBuilder<DateTimePicker<TValue>, DefaultProp,  TValue>() { Receiver = receiver };
        }

        public RenderFragment BuildDropdown(DropdownOptions options)
        {
            throw new NotImplementedException();
        }

        public RenderFragment BuildForm<TData>(FormOptions<TData> options) where TData : class, new()
        {
            throw new NotImplementedException();
        }

        public IBindableInputComponent<DefaultProp, TValue> BuildInput<TValue>(object receiver)
        {
            throw new NotImplementedException();
        }

        public RenderFragment BuildLoginForm(LoginFormModel model, Func<Task> handleLogin)
        {
            throw new NotImplementedException();
        }

        public RenderFragment BuildMenu(IRouterStore router, bool horizontal, IAppStore app)
        {
            throw new NotImplementedException();
        }

        public IUIComponent<ModalProp> BuildModal()
        {
            throw new NotImplementedException();
        }

        public IBindableInputComponent<DefaultProp, TValue> BuildNumberInput<TValue>(object receiver)
        {
            throw new NotImplementedException();
        }

        public IBindableInputComponent<DefaultProp, string> BuildPassword(object receiver)
        {
            throw new NotImplementedException();
        }

        public RenderFragment BuildPopover(PopoverOptions options)
        {
            throw new NotImplementedException();
        }

        public ISelectInput<SelectProp, TItem, TValue> BuildRadioGroup<TItem, TValue>(object receiver, IEnumerable<TItem> options)
        {
            throw new NotImplementedException();
        }

        public IUIComponent BuildRow()
        {
            throw new NotImplementedException();
        }

        public IBindableInputComponent<SelectProp, TValue> BuildSelect<TValue>(object receiver, SelectItem<TValue>? options)
        {
            throw new NotImplementedException();
        }

        public ISelectInput<SelectProp, TItem, TValue> BuildSelect<TItem, TValue>(object receiver, IEnumerable<TItem> options)
        {
            throw new NotImplementedException();
        }

        public IBindableInputComponent<SwitchProp, bool> BuildSwitch(object receiver)
        {
            throw new NotImplementedException();
        }

        public RenderFragment BuildTable<TModel, TQuery>(TableOptions<TModel, TQuery> options) where TQuery : IRequest, new()
        {
            throw new NotImplementedException();
        }

        public IBindableInputComponent<DefaultProp, string[]> BuildTree<TData>(object revicer, TreeOptions<TData> options)
        {
            throw new NotImplementedException();
        }

        public void Message(MessageType type, string message)
        {
            throw new NotImplementedException();
        }

        public void Notify(MessageType type, string title, string message)
        {
            throw new NotImplementedException();
        }

        public Task<TReturn> ShowDialogAsync<TReturn>(FlyoutOptions<TReturn> options)
        {
            throw new NotImplementedException();
        }

        public Task<TReturn> ShowDrawerAsync<TReturn>(FlyoutDrawerOptions<TReturn> options)
        {
            throw new NotImplementedException();
        }
    }
}
