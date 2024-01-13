using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
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
                return Task.FromResult(options.Validate());
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
            builder.OpenComponent<CascadingValue<bool>>(0);
            builder.AddAttribute(1, nameof(CascadingValue<bool>.Value), Edit);
            builder.AddAttribute(2, nameof(CascadingValue<bool>.ChildContent), UI.BuildForm(options));
            builder.CloseComponent();
        }
    }
}
