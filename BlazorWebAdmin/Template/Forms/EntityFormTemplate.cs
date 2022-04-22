using AntDesign;
using MT.KitTools.Mapper;

namespace BlazorWebAdmin.Template.Forms
{
    public class EntityFormTemplate<TEntity> : FeedbackComponent<TEntity, TEntity> where TEntity : class, new()
    {
        protected TEntity value;
        protected bool edit;

        protected override void OnInitialized()
        {
            base.OnInitialized();
            if (Options == null)
            {
                value = new TEntity();
                edit = false;
            }
            else
            {
                value = Mapper.Map<TEntity, TEntity>(Options);
                edit = true;
            }
        }

        public override async Task OnFeedbackOkAsync(ModalClosingEventArgs args)
        {
            await (FeedbackRef as IOkCancelRef<TEntity>)!.OkAsync(value);
            await base.OnFeedbackOkAsync(args);
        }
    }
}
