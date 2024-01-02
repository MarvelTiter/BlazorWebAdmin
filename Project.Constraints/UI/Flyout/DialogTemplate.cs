using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using MT.Toolkit.Mapper;
using Project.Constraints.Page;

namespace Project.Constraints.UI.Flyout
{
    public class FormParam<TEntity>
    {
        public TEntity? Value { get; set; }
        public bool Edit { get; set; }
        public FormParam(TEntity? entity, bool? edit)
        {
            Value = entity;
            Edit = edit ?? false;
        }

    }
    public class DialogTemplate<TValue> : BasicComponent, IFeedback<TValue>
    {
        [Parameter] public FormParam<TValue> DialogModel { get; set; }
        [Parameter] public RenderFragment ChildContent { get; set; }
        protected TValue? Value
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
            var valueType = typeof(TValue);
            if (DialogModel.Value == null && valueType.IsClass && valueType != typeof(string))
            {
                DialogModel.Value = (TValue)Activator.CreateInstance(valueType)!;
            }
            else
            {
                if (DialogModel.Value != null)
                    DialogModel.Value = Mapper.Map<TValue, TValue>(DialogModel.Value);
            }
        }

        public virtual Task<bool> OnPostAsync()
        {
            return Task.FromResult(true);
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
                builder.AddContent(0, ChildContent);
            }
        }
    }
}
