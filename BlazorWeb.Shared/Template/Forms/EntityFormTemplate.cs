using AntDesign;
using MT.KitTools.Mapper;

namespace BlazorWeb.Shared.Template.Forms
{
    public class EntityFormTemplate<TEntity> : FeedbackComponent<TEntity, TEntity> where TEntity : class, new()
    {
        protected TEntity Value;
        protected bool edit;
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

        protected virtual Task OnPostAsync()
        {
            return Task.CompletedTask;
        }

        public override async Task OnFeedbackOkAsync(ModalClosingEventArgs args)
        {
            await OnPostAsync();
            await (FeedbackRef as IOkCancelRef<TEntity>)!.OkAsync(Value);
            await base.OnFeedbackOkAsync(args);
        }
    }
}
