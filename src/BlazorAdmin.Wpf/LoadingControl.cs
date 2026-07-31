using AutoInjectGenerator;

namespace BlazorAdmin.Wpf;
[AutoInjectSelf]
internal class LoadingControl
{
    public bool HeaderLoaded { get; set; }
    public Action? Update { get; set; }
    public void Reset()
    {
        HeaderLoaded = false;
    }
}
