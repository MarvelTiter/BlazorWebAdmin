using Project.Constraints.UI.Builders;
using Project.Constraints.UI.Props;
using System.Linq.Expressions;

namespace Project.Constraints.UI.Extensions;

public static class SelectBuilderExtensions
{

    public static ISelectInput<SelectProp, KeyValuePair<string, string>, string> BuildDictionarySelect(this IUIService service, object receiver, Dictionary<string, string> options)
    {
        return service.BuildSelect<KeyValuePair<string, string>, string>(receiver, options.ToList()).LabelExpression(kv => kv.Value).ValueExpression(kv => kv.Key);
    }

    public static ISelectInput<SelectProp, KeyValuePair<TValue, string>, TValue> BuildDictionarySelect<TValue>(this IUIService service, object receiver, Dictionary<TValue, string> options) where TValue : notnull
    {
        return service.BuildSelect<KeyValuePair<TValue, string>, TValue>(receiver, options.ToList()).LabelExpression(kv => kv.Value).ValueExpression(kv => kv.Key);
    }
    public static ISelectInput<SelectProp, TItem, TValue> Multiple<TItem, TValue>(this ISelectInput<SelectProp, TItem, TValue> sel)
    {
        sel.SetModel(p => p.Mulitple = true);
        return sel;
    }
    public static ISelectInput<SelectProp, TItem, TValue> LabelExpression<TItem, TValue>(this ISelectInput<SelectProp, TItem, TValue> sel, Expression<Func<TItem, string>> expression)
    {
        sel.Set(s => s.LabelExpression, expression);
        return sel;
    }

    public static ISelectInput<SelectProp, TItem, TValue> ValueExpression<TItem, TValue>(this ISelectInput<SelectProp, TItem, TValue> sel, Expression<Func<TItem, TValue>> expression)
    {
        sel.Set(s => s.ValueExpression, expression);
        return sel;
    }

    public static ISelectInput<SelectProp, TItem, TValue[]> LabelsExpression<TItem, TValue>(this ISelectInput<SelectProp, TItem, TValue[]> sel, Expression<Func<TItem, string>> expression)
    {
        sel.Set(s => s.LabelExpression, expression);
        return sel;
    }

    public static ISelectInput<SelectProp, TItem, TValue[]> ValuesExpression<TItem, TValue>(this ISelectInput<SelectProp, TItem, TValue[]> sel, Expression<Func<TItem, TValue>> expression)
    {
        sel.Set(s => s.ValueExpression, expression);
        return sel;
    }
}

// public static class InputBuilderExtensions
// {
//     public static IBindableInputComponent<DefaultProp, string> BuildInput(this IUIService service, object receiver)
//     {
//         return service.BuildInput<string>(receiver);
//     }
// }

//public static class SelectPropExtensions
//{
//    public static IBindableInputComponent<SelectProp, string[]> LabelExpression<TItem>(this IBindableInputComponent<SelectProp, string[]> builder, Expression<Func<TItem, string>> expression)
//    {
//        return builder.Set(p => p.LabelExpression, expression);
//    }

//    public static IBindableInputComponent<SelectProp, string[]> ValueExpression<TItem, TValue>(this IBindableInputComponent<SelectProp, string[]> builder, Expression<Func<TItem, TValue>> expression)
//    {
//        return builder.Set(p => p.ValueExpression, expression);
//    }

//    public static IBindableInputComponent<SelectProp, TValue> ValueExpression<TItem, TValue>(this IBindableInputComponent<SelectProp, TValue> builder, Expression<Func<TItem, TValue>> expression)
//    {
//        return builder.Set(p => p.ValueExpression, expression);
//    }
//}