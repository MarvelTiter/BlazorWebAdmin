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

    public static IButtonInput Danger(this IButtonInput btn)
    {
        return btn.SetButtonType(ButtonType.Danger);
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

public static class SwitchBuilderExtensions
{
    public static IBindableInputComponent<SwitchProp, bool> CheckLabel(this IBindableInputComponent<SwitchProp, bool> component, string label)
    {
        component.Set(p => p.CheckedLabel, label);
        return component;
    }

    public static IBindableInputComponent<SwitchProp, bool> UnCheckLabel(this IBindableInputComponent<SwitchProp, bool> component, string label)
    {
        component.Set(p => p.UnCheckedLabel, label);
        return component;
    }
}