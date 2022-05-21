using AntDesign;
using BlazorWebAdmin.Template.Tables.Setting;
using Microsoft.AspNetCore.Components;
using MT.KitTools.Mapper;

namespace BlazorWebAdmin.Template.Forms
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

        public override async Task OnFeedbackOkAsync(ModalClosingEventArgs args)
        {
            await (FeedbackRef as IOkCancelRef<TEntity>)!.OkAsync(Value);
            await base.OnFeedbackOkAsync(args);
        }
    }
}
