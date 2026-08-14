using BlazorTemplate.ClientCore.UI;

namespace BlazorTemplate.ClientCore.Services;

public interface ILoginPage
{
    Task HandleLogin(LoginFormModel model);
    IUIService UI { get; }
}
