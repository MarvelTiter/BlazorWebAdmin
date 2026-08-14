namespace BlazorTemplate.ClientCore.Components;

public partial class FullScreen : JsComponentBase
{
    protected override async ValueTask Init()
    {
        await InvokeVoidAsync("init");
    }

    public async ValueTask Toggle()
    {
        await InvokeVoidAsync("toggle");
    }
}