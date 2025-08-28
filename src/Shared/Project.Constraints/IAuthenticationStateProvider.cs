using Project.Constraints.Services;

namespace Project.Constraints;

public interface IAuthenticationStateProvider
{
    IAuthService? AuthService { get; }
    UserInfo? Current {  get; }
    // Task IdentifyUser(UserInfo info);
    Task ClearState();
}
