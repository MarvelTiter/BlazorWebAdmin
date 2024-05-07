using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Project.Constraints.UI.Extensions;
using Project.Constraints.UI.Form;
using Project.Constraints.UI.Table;

namespace Project.Constraints.UI.Flyout
{
    public sealed class FormDialogTemplate<TValue> : DialogTemplate<TValue> where TValue : class, new()
    {
        [Parameter] public IEnumerable<ColumnInfo> Columns { get; set; }
        FormOptions<TValue>? options;
        public override Task<bool> OnPostAsync()
        {
            if (options != null && options.Validate != null)
            {
               var flag = Options.PostCheck?.Invoke(Value, options.Validate) ?? options.Validate.Invoke();
                return Task.FromResult(flag);
            }
            return base.OnPostAsync();
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
}
