using AntDesign;
using Microsoft.AspNetCore.Components;
using Project.AppCore.UI;
using Project.Models;

namespace Project.UI.AntBlazor
{

    public class UIService : IUIService
    {
        public IBindableInput<InputInfo<string>, string> BuildInput(object reciver)
        {
            return new BindableComponentBuilder<Input<string>, InputInfo<string>, string>() { Reciver = reciver };
        }

        public IBindableInput<InputInfo<TValue>, TValue> BuildInput<TValue>(object reciver)
        {
            return new BindableComponentBuilder<InputNumber<TValue>, InputInfo<TValue>, TValue>() { Reciver = reciver };
        }

        public IBindableInput<InputInfo<string>, string> BuildPassword(object reciver)
        {
            return new BindableComponentBuilder<InputPassword, InputInfo<string>, string>() { Reciver = reciver };
        }

        public IBindableInput<SelectInfo<TValue>, TValue> BuildSelect<TValue>(object reciver, SelectItem<TValue> options)
        {
            return new BindableComponentBuilder<Select<TValue, Options<TValue>>, SelectInfo<TValue>, TValue>() { Reciver = reciver }
                .Set(c => c.DataSource, options)
                .Set("ValueName", "Value")
                .Set("LabelName", "Label");
        }
    }
}
