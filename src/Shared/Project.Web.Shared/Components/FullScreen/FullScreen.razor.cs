namespace Project.Web.Shared.Components
{
    public partial class FullScreen : JsComponentBase
    {
        protected override async ValueTask Init()
        {
            await ModuleInvokeVoidAsync("init");
        }

        public async ValueTask Toggle()
        {
            await ModuleInvokeVoidAsync("toggle");
        }
    }
}
