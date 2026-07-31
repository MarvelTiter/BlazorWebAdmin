using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using BlazorTemplate.Constraints.UI.Extensions;
using BlazorTemplate.Constraints.UI.Form;
using BlazorTemplate.Constraints.UI.Table;

namespace BlazorTemplate.Constraints.UI.Flyout;

public sealed class FormDialogTemplate<TValue> : DialogTemplate<TValue> where TValue : class, new()
{
    [Parameter, NotNull] public IEnumerable<ColumnInfo>? Columns { get; set; }
    FormOptions<TValue>? options;
    public override async Task<bool> OnPostAsync()
    {
        if (options?.Validate == null) return await base.OnPostAsync();
        if (Options.PostCheckAsync == null) return options.Validate.Invoke();
        var checke = await Options.PostCheckAsync(Value,options.Validate);
        return checke;
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        options = new FormOptions<TValue>(UI, Value, Columns.ToList());
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (options == null) return;
        builder.Component<CascadingValue<bool>>()
            .SetComponent(c => c.Value, Edit)
            .SetComponent(c => c.ChildContent, UI.BuildForm(options))
            .Build();
    }
}