//using AntDesign;
//using Microsoft.AspNetCore.Components;
//using Microsoft.Extensions.Localization;
//using MT.Toolkit.Mapper;

//namespace Project.Web.Shared.Template.Forms
//{
//    public class FormParam<TEntity>
//    {
//        public TEntity? Value { get; set; }
//        public bool Edit { get; set; }
//        public FormParam(TEntity? entity, bool edit)
//        {
//            Value = entity;
//            Edit = edit;
//        }

//        public FormParam(TEntity? entity) : this(entity, entity != null)
//        {

//        }
//    }
//    public class EntityFormTemplate<TEntity> : FeedbackComponent<FormParam<TEntity>, TEntity> where TEntity : class, new()
//    {
//        protected TEntity Value => Options.Value!;
//        protected bool Edit => Options.Edit;
//        [Inject] protected IStringLocalizer<TEntity> Localizer { get; set; }

//        protected string GetLocalizeString(string prop) => Localizer[$"{typeof(TEntity).Name}.{prop}"];

//        protected override void OnInitialized()
//        {
//            if (Options.Value == null)
//            {
//                Options.Value = new TEntity();
//            }
//            else
//            {
//                Options.Value = Mapper.Map<TEntity, TEntity>(Options.Value);
//            }
//            base.OnInitialized();
//        }

//        protected virtual Task<bool> OnPostAsync()
//        {
//            return Task.FromResult(true);
//        }

//        public override async Task OnFeedbackOkAsync(ModalClosingEventArgs args)
//        {
//            bool result = true;
//            if (FeedbackRef is ModalRef<TEntity> modalRef)
//            {
//                modalRef.Config.ConfirmLoading = true;
//                await modalRef.UpdateConfigAsync();
//                result = await OnPostAsync();
//                modalRef.Config.ConfirmLoading = false;
//            }
//            if (result)
//            {
//                await OkCancelRefWithResult.OkAsync(Value);
//            }
//            else
//            {
//                args.Reject();
//            }
//        }
//    }
//}
