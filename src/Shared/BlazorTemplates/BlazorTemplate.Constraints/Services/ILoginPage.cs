using BlazorTemplate.Constraints.UI;

namespace BlazorTemplate.Constraints.Services;

public interface ILoginPage
{
    Task HandleLogin(LoginFormModel model);
    IUIService UI { get; }
}
