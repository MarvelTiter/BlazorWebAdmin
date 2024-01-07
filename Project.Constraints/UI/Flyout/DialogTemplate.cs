﻿using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using MT.Toolkit.Mapper;
using Project.Constraints.Page;
using Project.Constraints.UI.Form;
using Project.Constraints.UI.Table;

namespace Project.Constraints.UI.Flyout
{
    public class FormParam<TEntity>(TEntity? entity, bool? edit)
    {
        public TEntity? Value { get; set; } = entity;
        public bool Edit { get; set; } = edit ?? entity != null;
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
            if (DialogModel == null) return;
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
                builder.OpenComponent<CascadingValue<bool>>(0);
                builder.AddAttribute(1, nameof(CascadingValue<bool>.Value), Edit);
                builder.AddAttribute(2, nameof(CascadingValue<bool>.ChildContent), ChildContent);
                builder.CloseComponent();
            }
        }
    }

    public sealed class FormDialogTemplate<TValue> : DialogTemplate<TValue> where TValue : class, new()
    {
        [Parameter] public IEnumerable<ColumnInfo> Columns { get; set; }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            var options = new FormOptions<TValue>(UI, Value, Columns.ToList());
            ChildContent = UI.BuildForm(options);
            base.BuildRenderTree(builder);
        }
    }
}