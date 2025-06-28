using Microsoft.AspNetCore.Components;
using Project.Constraints.UI.Props;

namespace Project.Constraints.UI.Extensions;

public static class ButtonBuilderExtensions
{
    public static IButtonInput Text(this IButtonInput btn, string text)
    {
        //btn.s
        btn.Set("ChildContent", text.AsContent());
        return btn;
    }

    public static IButtonInput Primary(this IButtonInput btn)
    {
        return btn.SetButtonType(ButtonType.Primary);
    }

    public static IButtonInput SetButtonType(this IButtonInput btn, ButtonType type)
    {
        btn.Set(p => p.ButtonType, type);
        return btn;
    }
}

public static class CheckboxBuilderExtensions
{
    public static IBindableInputComponent<DefaultProp, bool> Text(this IBindableInputComponent<DefaultProp, bool> component, string label)
    {
        component.Set(p => p.Label, label);
        return component;
    }
}