using BlazorTemplate.Constraints.Services;

namespace BlazorTemplate.Constraints;

public interface IAuthenticationStateProvider
{
    IAuthService? AuthService { get; }
    UserInfo? Current {  get; }
    // Task IdentifyUser(UserInfo info);
    Task ClearState();
}
