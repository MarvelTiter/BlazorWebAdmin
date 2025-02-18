using Project.Constraints.UI;

namespace Project.Constraints.Services;

public interface ILoginPage
{
    Task HandleLogin(LoginFormModel model);
    IUIService UI { get; }
}
