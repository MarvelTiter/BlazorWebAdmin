using System.Diagnostics.CodeAnalysis;
using BlazorTemplate.ClientCore.Utils;
using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;

namespace BlazorTemplate.UI.FluentUI.Components;

#pragma warning disable CS8714
public sealed partial class FluentEnumSelect<TEnum> : FluentSelect<TEnum, TEnum>
{
    public FluentEnumSelect(LibraryConfiguration configuration) : base(configuration) { }

    [Parameter] public TEnum? EnumValue { get; set; }
    [Parameter] public EventCallback<TEnum?> EnumValueChanged { get; set; }

    public override IEnumerable<TEnum>? Items { get => EnumHelper<TEnum>.GetValues(); set => throw new NotImplementedException(); }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        OptionText = EnumHelper<TEnum>.GetDisplayName;
        OptionValue = e => e!;
        Value = EnumValue!;
        ValueChanged = EventCallback.Factory.Create<TEnum>(this, NotifyChanged);
    }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        Value = EnumValue!;
    }

    private async Task NotifyChanged(TEnum value)
    {
        await EnumValueChanged.InvokeAsync(value);
    }
}

public sealed class FluentTypedSelect<TItem, TValue> : FluentSelect<TItem, TValue> where TItem : notnull
{
    public FluentTypedSelect(LibraryConfiguration configuration) : base(configuration) { }

    [Parameter] public TValue? SelectedValue { get; set; }
    [Parameter] public EventCallback<TValue> SelectedValueChanged { get; set; }
    [Parameter, NotNull] public Func<TItem, TValue>? ItemValue { get; set; }
    [Parameter, NotNull] public Func<TItem, string>? ItemLabel { get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        OptionText = ItemLabel;
        OptionValue = ItemValue;
        Value = SelectedValue!;
        ValueChanged = EventCallback.Factory.Create<TValue>(this, NotifyChanged);
    }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        Value = SelectedValue!;
    }

    private async Task NotifyChanged(TValue value)
    {
        await SelectedValueChanged.InvokeAsync(value);
    }
}
