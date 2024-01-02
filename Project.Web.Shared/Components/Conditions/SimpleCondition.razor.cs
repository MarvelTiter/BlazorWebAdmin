using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Project.Web.Shared.Extensions;

namespace Project.Web.Shared.Components
{
    public partial class SimpleCondition<TData> : ConditionBase
    {
        RenderFragment CreateBody()
        {
            return builder =>
            {
                builder.MakeDiv(() =>
                {
                    builder.OpenElement(0, "span");
                    builder.AddAttribute(1,"class", "conditions");
                    builder.AddAttribute(2, "style", Style);
                    builder.OpenElement(3, "span");
                    builder.AddAttribute(4, "class", "label");
                    builder.AddAttribute(5, "style", $"{Parent.LabelWidth}px");
                    builder.AddContent(6, Label);
                    builder.CloseElement();

                    if (typeof(TData) == typeof(DateTime))
                    {
                        builder.AddContent(7, UI.BuildDatePicker<TData>(this).Bind(() => Value).Render());
                    }
                    else if (EnumValues != null)
                    {
                        //builder.OpenComponent<DictionarySelect>(7);
                        //builder.AddComponentParameter(8, "Options", EnumValues);
                        //builder.AddComponentParameter(9, "Value", stringValue);
                        //builder.AddComponentParameter(10, "ValueChanged", EventCallback.Factory.Create<string>(this, s => stringValue = s));
                        UI.BuildSelect<KeyValuePair<string, string>, string>(this, EnumValues!.ToList()).LabelExpression(kv => kv.Key).ValueExpression(kv => kv.Value).Bind(() => stringValue).Render().Invoke(builder);
                    }
                    else
                    {
                        builder.AddContent(7, UI.BuildInput<TData>(this).Bind(() => Value).Render());
                    }


                    builder.CloseElement();
                    
                });
            };
        }
    }
}
