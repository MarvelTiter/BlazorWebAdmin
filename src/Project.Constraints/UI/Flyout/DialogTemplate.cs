using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.Extensions.Localization;
using MT.Toolkit.Mapper;
using Project.Constraints.Page;
using Project.Constraints.UI.Extensions;
using System.Diagnostics.CodeAnalysis;

namespace Project.Constraints.UI.Flyout
{
    public class FormParam<TEntity>(TEntity? entity, bool? edit)
    {
        public TEntity? Value { get; set; } = entity;
        public bool Edit { get; set; } = edit ?? entity != null;
    }
    public class DialogTemplate<TValue> : JsComponentBase, IFeedback<TValue>
    {
        [Parameter, NotNull] public FormParam<TValue> DialogModel { get; set; }
        [Parameter, NotNull] public RenderFragment ChildContent { get; set; }
        [Parameter, NotNull] public FlyoutOptions<TValue> Options { get; set; }
        [Inject] protected IStringLocalizer<TValue> Localizer { get; set; }
        protected string GetLocalizeString(string prop) => Localizer[$"{typeof(TValue).Name}.{prop}"];

        protected TValue Value
        {
            get
            {
                return DialogModel.Value;
            }
            set
            {
                DialogModel.Value = value;
            }
        }

        protected bool Edit => DialogModel.Edit;
        protected override void OnInitialized()
        {
            base.OnInitialized();
            LoadJs = false;
            var valueType = typeof(TValue);
            if (DialogModel == null) return;
            if (DialogModel.Value == null && valueType.IsClass && valueType != typeof(string))
            {
                DialogModel.Value = (TValue)Activator.CreateInstance(valueType)!;
            }
            else
            {
                if (valueType.IsClass && valueType != typeof(string) && DialogModel.Value != null)
                    DialogModel.Value = Mapper.Map<TValue, TValue>(DialogModel.Value);
            }
        }

        public virtual Task<bool> OnPostAsync()
        {
            return Task.FromResult(true);
        }

        protected Task CloseAsync()
        {
            return Options.OnClose();
        }

        protected Task OkAsync()
        {
            return Options.OnOk();
        }

        public Task OnCancelAsync()
        {
            return Task.CompletedTask;
        }

        public async Task<FeedBackValue<TValue>> OnOkAsync()
        {
            var flag = await OnPostAsync();
            return new FeedBackValue<TValue> { Value = Value, Success = flag };
        }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            if (ChildContent != null)
            {
                builder.Component<CascadingValue<bool>>()
                    .SetComponent(c => c.Value, Edit)
                    .SetComponent(c => c.ChildContent, ChildContent)
                    .Build();
            }
        }
    }
}
