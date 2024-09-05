using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.FluentUI.AspNetCore.Components;
using MT.Toolkit.ReflectionExtension;
using Project.Constraints.Utils;

namespace Project.UI.FluentUI.Components;

#pragma warning disable CS8714
public sealed partial class FluentEnumSelect<TEnum> : FluentSelect<TEnum>
{
    [Parameter] public TEnum? EnumValue { get; set; }
    [Parameter] public EventCallback<TEnum?> EnumValueChanged { get; set; }
    public override IEnumerable<TEnum>? Items { get => EnumHelper<TEnum>.GetValues(); set => throw new NotImplementedException(); }
    
    protected override void OnInitialized()
    {
        base.OnInitialized();
        // OptionValue = EnumHelper<TEnum>.GetDisplayName;
        OptionText = EnumHelper<TEnum>.GetDisplayName;
        SelectedOption = EnumValue;
        SelectedOptionChanged = EnumValueChanged;
        //ValueChanged = EventCallback.Factory.Create(this, Action); 
        ValueExpression = () => $"{EnumValue}";
    }

    //Func<string?, Task> Action => NotifyChanged;

    //public async Task NotifyChanged(string? item)
    //{
    //    if (item == null)
    //        return;
    //    var e = EnumHelper<TEnum>.Parse(item);
    //    await EnumValueChanged.InvokeAsync(e);
    //}
}

public sealed class FluentTypedSelect<TItem, TValue> : FluentSelect<TItem> where TItem : notnull
{
    [Parameter] public TValue? SelectedValue { get; set; }
    [Parameter] public EventCallback<TValue> SelectedValueChanged { get; set; }
    [Parameter, NotNull] public Func<TItem, TValue>? ItemValue { get; set; }
    [Parameter, NotNull] public Func<TItem, string>? ItemLabel { get; set; }
    TItem? selectedOption;
    protected override void OnInitialized()
    {
        base.OnInitialized();
        OptionText = ItemLabel;
        OptionValue = ItemLabel;
        InitValue();
        SelectedOption = selectedOption;
        ValueExpression = () => $"{(selectedOption == null ? "" : ItemValue.Invoke(selectedOption))}";
        SelectedOptionChanged = EventCallback.Factory.Create(this, Action);
    }

    private void InitValue()
    {
        selectedOption = Items!.FirstOrDefault(i => Equals(ItemValue(i), SelectedValue));
        if (selectedOption == null) return;
        InternalValue = ItemValue.Invoke(selectedOption)?.ToString();
    }

    Func<TItem?, Task> Action => NotifyChanged;

    public async Task NotifyChanged(TItem? item)
    {
        if (item == null)
            return;
        var val = ItemValue.Invoke(item);
        await SelectedValueChanged.InvokeAsync(val);
    }
}