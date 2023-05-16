using AntDesign;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using MT.Toolkit.Mapper;

namespace BlazorWeb.Shared.Template.Forms
{
    public class EntityFormTemplate<TEntity> : FeedbackComponent<TEntity, TEntity> where TEntity : class, new()
    {
        protected TEntity Value;
        protected bool edit;
        [Inject] protected IStringLocalizer<TEntity> Localizer { get; set; }
        protected override void OnInitialized()
        {
            base.OnInitialized();
            if (Options == null)
            {
                Value = new TEntity();
                edit = false;
            }
            else
            {
                Value = Mapper.Map<TEntity, TEntity>(Options);
                edit = true;
            }
        }

        protected virtual Task<bool> OnPostAsync()
        {
            return Task.FromResult(true);
        }

        public override async Task OnFeedbackOkAsync(ModalClosingEventArgs args)
        {
            bool result = true;
            if (FeedbackRef is ModalRef<TEntity> modalRef)
            {
                modalRef.Config.ConfirmLoading = true;
                await modalRef.UpdateConfigAsync();
                result = await OnPostAsync();
                modalRef.Config.ConfirmLoading = false;                
            }
            if (result)
            {
                await OkCancelRefWithResult.OkAsync(Value);
            }
            else
            {
                args.Reject();
            }
        }
    }
}
