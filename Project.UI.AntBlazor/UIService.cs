using AntDesign;
using Microsoft.AspNetCore.Components;
using Project.AppCore.UI;
using Project.AppCore.UI.Table;
using Project.Models;
using Project.Models.Request;
using Project.UI.AntBlazor.Components;

namespace Project.UI.AntBlazor
{

    public class UIService : IUIService
    {

        public IBindableInput<string> BuildInput(object reciver)
        {
            return new BindableComponentBuilder<Input<string>, string>() { Reciver = reciver };
        }

        public IBindableInput<TValue> BuildInput<TValue>(object reciver)
        {
            return new BindableComponentBuilder<InputNumber<TValue>, TValue>() { Reciver = reciver };
        }

        public IBindableInput<string> BuildPassword(object reciver)
        {
            return new BindableComponentBuilder<InputPassword, string>() { Reciver = reciver };
        }

        public IBindableInput<TValue> BuildSelect<TValue>(object reciver, SelectItem<TValue> options)
        {
            return new BindableComponentBuilder<Select<TValue, Options<TValue>>, TValue>() { Reciver = reciver }
                .Set("DataSource", options)
                .Set("ValueName", "Value")
                .Set("LabelName", "Label");
        }
        public IButtonAction BuildButton(object reciver)
        {
            return new ButtonBuilder<Button>() { Reciver = reciver };
        }

        public void Message(AppCore.UI.MessageType type, string message)
        {
            throw new NotImplementedException();
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
    }
}
