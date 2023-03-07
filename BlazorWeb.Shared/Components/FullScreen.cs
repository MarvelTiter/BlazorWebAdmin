namespace BlazorWeb.Shared.Components
{
    public class FullScreen : JsComponentBase
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
